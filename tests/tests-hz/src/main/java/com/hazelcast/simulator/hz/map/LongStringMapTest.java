/*
 * Copyright (c) 2008-2016, Hazelcast, Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.hazelcast.simulator.hz.map;

import com.hazelcast.core.IMap;
import com.hazelcast.simulator.hz.HazelcastTest;
import com.hazelcast.simulator.test.BaseThreadState;
import com.hazelcast.simulator.test.annotations.Prepare;
import com.hazelcast.simulator.test.annotations.Setup;
import com.hazelcast.simulator.test.annotations.Teardown;
import com.hazelcast.simulator.test.annotations.TimeStep;

import java.util.Random;

import static com.hazelcast.simulator.tests.helpers.HazelcastTestUtils.getOperationCountInformation;
import static com.hazelcast.simulator.utils.GeneratorUtils.generateStrings;

public class LongStringMapTest extends HazelcastTest {

    // properties
    public int keyCount = 10000;
    public int valueCount = 10000;
    public int valueLength = 10;
    public int minValueLength = valueLength;
    public int maxValueLength = valueLength;

    private IMap<Long, String> map;
    private String[] values;

    @Setup
    public void setUp() {
        map = targetInstance.getMap(name);
    }

    @Prepare(global = true)
    public void prepare() {
        values = generateStrings(valueCount, minValueLength, maxValueLength);

        Random random = new Random();
        for (long key = 0; key < keyCount; key++) {
            String value = values[random.nextInt(valueCount)];
            map.put(key, value);
        }
    }

    @TimeStep(prob = -1)
    public String get(ThreadState state) {
        return map.get(state.randomKey());
    }

    @TimeStep(prob = 0.1)
    public String put(ThreadState state) {
        return map.put(state.randomKey(), state.randomValue());
    }

    @TimeStep(prob = 0)
    public void set(ThreadState state) {
        map.set(state.randomKey(), state.randomValue());
    }

    public class ThreadState extends BaseThreadState {

        private long randomKey() {
            return randomLong(keyCount);
        }

        private String randomValue() {
            return values[randomInt(values.length)];
        }
    }

    @Teardown
    public void tearDown() {
        map.destroy();
        logger.info(getOperationCountInformation(targetInstance));
    }
}
