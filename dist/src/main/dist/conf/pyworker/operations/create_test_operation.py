import json


class CreateTestOperation:
    def __init__(self, test_id, properties):
        self._test_id = test_id
        self._properties = properties

    def get_test_id(self):
        return self._test_id

    def get_properties(self):
        return self._properties

    @staticmethod
    def parse(input_string):
        payload = json.loads(input_string["payload"])
        print("===== Properties: " + str(payload))
        return CreateTestOperation(test_id=payload["testId"],
                                   properties=payload["properties"])

