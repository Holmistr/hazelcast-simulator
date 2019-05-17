import random

from decorators.prepare import prepare
from decorators.setup import setup
from decorators.timestep import timestep


class IntStringMapTest:

    def __init__(self):
        self.keyCount = []

        self._keys = []

    @setup
    def setup(self):
        print("Execute Setup method")

    @prepare
    def prepare(self):
        print("Execute Prepare method")

        for i in range(0, self.keyCount):
            self._keys.append(i)
            self._map.put(i, "value" + str(i))

    @timestep
    def get(self):
        print(self._map.get(self._random_key()))

    def _random_key(self):
        return self._keys[random.randrange(len(self._keys))]



