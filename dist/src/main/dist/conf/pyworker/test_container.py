import hazelcast

from int_string_map_test import IntStringMapTest
from performance.test_performance_tracker import TestPerformanceTracker
from time_step_thread import TimeStepThread


class TestContainer:
    def __init__(self, test_id, properties, test_context):
        self._test_id = test_id
        self._properties = properties
        self._test_context = test_context

        self._test_instance = IntStringMapTest()
        self._bind_properties()
        self._inject_driver()

        self._performance_tracker = TestPerformanceTracker()

    def invoke(self, test_phase, promise):
        if test_phase == "RUN":
            run_thread = TimeStepThread(self.get_timestep_method(),
                                        self._test_context,
                                        self._performance_tracker,
                                        promise)
            run_thread.start()
        elif test_phase == "SETUP":
            setup_method = self.get_setup_method()
            setup_method()
        elif test_phase == "LOCAL_PREPARE":
            prepare_method = self.get_prepare_method()
            prepare_method()

    def _bind_properties(self):
        for key, value in self._properties.items():
            has_attribute = hasattr(self._test_instance, key)
            if has_attribute:
                if value.isdigit():
                    value = int(value)
                setattr(self._test_instance, key, value)

    def get_test_context(self):
        return self._test_context

    def get_performance_tracker(self):
        return self._performance_tracker

    def get_setup_method(self):
        return self._get_annotated_method(self._test_instance, "_setup")

    def get_prepare_method(self):
        return self._get_annotated_method(self._test_instance, "_prepare")

    def get_timestep_method(self):
        return self._get_annotated_method(self._test_instance, "_timestep")

    def _get_annotated_method(self, object, annotation_marker):
        for attr in dir(object):
            method = getattr(object, attr)

            if hasattr(method, annotation_marker):
                return method

    def _inject_driver(self):
        config = hazelcast.ClientConfig()
        config.group_config.name = "workers"
        client = hazelcast.HazelcastClient(config)
        map = client.get_map("map").blocking()

        setattr(self._test_instance, "_map", map)


