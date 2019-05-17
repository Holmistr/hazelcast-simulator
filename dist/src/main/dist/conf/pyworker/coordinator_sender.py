class CoordinatorSender():
    def __init__(self, connection):
        self._connection = connection

    def send(self, payload):
        self._connection.send(body='ok', destination='/topic/coordinator', headers=payload, content_type='text/plain')


