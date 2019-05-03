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
package com.hazelcast.simulator.jedis;

import com.hazelcast.simulator.test.BaseThreadState;
import com.hazelcast.simulator.test.annotations.Prepare;
import com.hazelcast.simulator.test.annotations.Setup;
import com.hazelcast.simulator.test.annotations.Teardown;
import com.hazelcast.simulator.test.annotations.TimeStep;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.JedisPool;

import java.nio.ByteBuffer;
import java.util.HashSet;
import java.util.Random;
import java.util.Set;

import static com.hazelcast.simulator.utils.GeneratorUtils.generateByteArray;

public class ByteByteTest extends JedisTest {

    // properties
    public int keyCount = 1000;
    public int keyLength = 10;
    public int valueSize = 1000;

    private byte[][] keys;

    @Setup
    public void setUp() {
        Random random = new Random();
        keys = new byte[keyCount][];
        for (int i = 0; i < keys.length; i++) {
            byte[] keyBytes = ByteBuffer.allocate(keyLength).putInt(i).array();
            keys[i] = keyBytes;
        }
        System.out.println("Key space: " + keys.length);
    }

    @Prepare(global = true)
    public void prepare() {
        Random random = new Random();

        for (byte[] key : keys) {
            client.set(key, generateByteArray(random, valueSize));
        }
    }

    @TimeStep(prob = 0.1)
    public String put(ThreadState state) {
        return client.set(state.randomKey(), state.randomValue());
    }

    @TimeStep(prob = -1)
    public byte[] get(ThreadState state) {
        return client.get(state.randomKey());
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
//        client.shutdown();
    }

//    private void printSize() {
//        System.out.println("Number of cluster nodes: " + client.getClusterNodes().size());
//        int sum = 0;
//        Set<Object> keys = new HashSet<Object>();
//        for (JedisPool pool: client.getClusterNodes().values()) {
//            Jedis jedis = pool.getResource();
//            keys.addAll(jedis.keys("*"));
//            sum += jedis.dbSize();
//            jedis.close();
//        }
//
//        System.out.println("Key array size: " + this.keys.length);
//        int i = 0;
//        for (byte[] key: this.keys) {
//            System.out.println("Key " + i + ": " + key);
//            i++;
//        }
//
//        System.out.println("DB SIze: " + sum);
//        System.out.println("Unique keys: " + keys.size());
//
//    }

}
