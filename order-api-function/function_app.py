import azure.functions as func
import datetime
import json
import logging
from helpers import validate_order

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

    return func.HttpResponse(
        json.dumps({"message": "Order has been processed successfully"}), 
        status_code=200, 
        mimetype="application/json"
    )

    