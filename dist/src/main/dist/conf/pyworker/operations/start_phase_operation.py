import json


class StartPhaseOperation():
    def __init__(self, test_phase, test_id):
        self._test_phase = test_phase
        self._test_id = test_id

    def get_test_phase(self):
        return self._test_phase

    @staticmethod
    def parse(input_string):
        payload = json.loads(input_string["payload"])
        return StartPhaseOperation(test_phase=payload["testPhase"],
                          test_id=payload["testId"])

