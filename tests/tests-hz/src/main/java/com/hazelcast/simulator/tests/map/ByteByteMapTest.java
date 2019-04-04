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
package com.hazelcast.simulator.tests.map;

import com.hazelcast.core.IMap;
import com.hazelcast.simulator.hz.HazelcastTest;
import com.hazelcast.simulator.test.BaseThreadState;
import com.hazelcast.simulator.test.annotations.Prepare;
import com.hazelcast.simulator.test.annotations.Setup;
import com.hazelcast.simulator.test.annotations.Teardown;
import com.hazelcast.simulator.test.annotations.TimeStep;
import com.hazelcast.simulator.worker.loadsupport.Streamer;
import com.hazelcast.simulator.worker.loadsupport.StreamerFactory;

import java.util.Random;

import static com.hazelcast.simulator.utils.GeneratorUtils.generateByteArray;

public class ByteByteMapTest extends HazelcastTest {

    // properties
    public int keyCount = 1000;
    public int keyLength = 10;
    public int valueCount = 1000;
    public int valueSize = 1000;

    private IMap<Object, Object> map;
    private byte[][] keys;

    @Setup
    public void setUp() {
        map = targetInstance.getMap(name);
        Random random = new Random();
        keys = new byte[keyCount][];
        for (int i = 0; i < keys.length; i++) {
            keys[i] = generateByteArray(random, keyLength);
        }
    }

    @Prepare(global = true)
    public void prepare() {
        Random random = new Random();

        Streamer<Object, Object> streamer = StreamerFactory.getInstance(map);
        for (byte[] key : keys) {
            streamer.pushEntry(key, generateByteArray(random, valueSize));
            System.out.println("Pushing entry " + key + ": " + map.size());
        }
        streamer.await();

        System.out.println("Number of entries: " + map.size());
    }

    @TimeStep(prob = 0.1)
    public Object put(ThreadState state) {
        return map.put(state.randomKey(), state.randomValue());
    }

    @TimeStep(prob = -1)
    public Object get(ThreadState state) {
        return map.get(state.randomKey());
    }

    @TimeStep(prob = 0.1)
    public void set(ThreadState state) {
        map.set(state.randomKey(), state.randomValue());
    }

    public class ThreadState extends BaseThreadState {

        private byte[] randomKey() {
            return keys[randomInt(keys.length)];
        }

        private byte[] randomValue() {
            return generateByteArray(random, valueSize);
        }
    }

    @Teardown
    public void tearDown() {
        map.destroy();
    }
}
