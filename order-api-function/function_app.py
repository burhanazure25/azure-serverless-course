import os
import azure.functions as func
import datetime
import json
import logging
from helpers import validate_order
from azure.servicebus import ServiceBusClient, ServiceBusMessage

app = func.FunctionApp()

@app.route(route="OrderTrigger", methods=["POST"], auth_level=func.AuthLevel.FUNCTION)
def OrderTrigger(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Processing order request.')

    try:
        order = req.get_json()
    except ValueError:
        return func.HttpResponse("Invalid JSON payload", status_code=400)
    
    isvalid, message = validate_order(order)

    if not isvalid:
        return func.HttpResponse(
            json.dumps({"error": message}), 
            status_code=400,
            mimetype="application/json")
    
    # Get connection string for service bus
    sb_connection_str = os.getenv("SERVICE_BUS_CONNECTION_STRING")
    queue_name = os.getenv("QUEUE_NAME")

    try:
        with ServiceBusClient.from_connection_string(sb_connection_str) as client:
            sender = client.get_queue_sender(queue_name=queue_name)
            with sender:
                message = ServiceBusMessage(json.dumps(order))
                sender.send_messages(message)

            return func.HttpResponse(
                json.dumps({"message": "Order has been processed successfully"}), 
                status_code=200, 
                mimetype="application/json"
            )

    except Exception as e:
        logging.error(f"Error sending message on the queue: {e}")
        return func.HttpResponse(
            json.dumps({"error": f"Service bus issue: {str(e)}"}), 
            status_code=500, 
            mimetype="application/json"
        )