class ResponsePromise():
    def __init__(self, connection, correlation_id, reply_to):
        self._connection = connection
        self._correlation_id = correlation_id
        self._reply_to = reply_to

    def reply_ok(self):
        response_headers = {"correlation-id": self._correlation_id, "payload": "ok"}
        self._connection.send(destination=self._reply_to, body="ok", headers=response_headers)

