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
package com.hazelcast.simulator.tests.concurrent.atomiclong;

import com.hazelcast.core.DistributedObject;
import com.hazelcast.core.ExecutionCallback;
import com.hazelcast.cp.IAtomicLong;
import com.hazelcast.executor.impl.ExecutionCallbackAdapter;
import com.hazelcast.simulator.hz.HazelcastTest;
import com.hazelcast.simulator.probes.Probe;
import com.hazelcast.simulator.test.BaseThreadState;
import com.hazelcast.simulator.test.StopException;
import com.hazelcast.simulator.test.annotations.AfterRun;
import com.hazelcast.simulator.test.annotations.BeforeRun;
import com.hazelcast.simulator.test.annotations.Setup;
import com.hazelcast.simulator.test.annotations.StartNanos;
import com.hazelcast.simulator.test.annotations.Teardown;
import com.hazelcast.simulator.test.annotations.TimeStep;
import com.hazelcast.simulator.test.annotations.Verify;
import com.hazelcast.simulator.tests.helpers.KeyLocality;
import com.hazelcast.simulator.utils.AssertTask;
import com.hazelcast.simulator.utils.ExceptionReporter;

import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

import static com.hazelcast.simulator.tests.helpers.HazelcastTestUtils.isClient;
import static com.hazelcast.simulator.tests.helpers.HazelcastTestUtils.isMemberNode;
import static com.hazelcast.simulator.tests.helpers.HazelcastTestUtils.rethrow;
import static com.hazelcast.simulator.tests.helpers.KeyUtils.generateStringKeys;
import static com.hazelcast.simulator.utils.CommonUtils.sleepSeconds;
import static com.hazelcast.simulator.utils.TestUtils.assertTrueEventually;
import static org.junit.Assert.assertEquals;

public class AsyncAtomicLongTest extends HazelcastTest {

    // properties
    public KeyLocality keyLocality = KeyLocality.SHARED;
    public int countersLength = 1000;
    public int assertEventuallySeconds = 300;
    // infinite batch size. If no batch-size is set, one can easily overload the system with requests.
    // unless backpressure is enabled.
    public int batchSize = -1;

    private IAtomicLong totalCounter;
    private IAtomicLong[] counters;

    @Setup
    public void setup() {
        totalCounter = getAtomicLong(name + ":TotalCounter");
        if (isMemberNode(targetInstance)) {
            counters = new IAtomicLong[countersLength];

            String[] names = generateStringKeys(name, countersLength, keyLocality, targetInstance);
            for (int i = 0; i < countersLength; i++) {
                counters[i] = getAtomicLong(names[i]);
            }
        }
    }

    @BeforeRun
    public void beforeRun() {
        // once this test is converted to a 3.7+ test, we can get rid of this since it will be supported on the clients.
        if (isClient(targetInstance)) {
            throw new StopException();
        }
    }

    @TimeStep
    public void write(ThreadState state, Probe probe, @StartNanos long startNanos) {
        IAtomicLong counter = state.getRandomCounter();
        state.increments++;
        CompletableFuture<Long> future = counter.incrementAndGetAsync().toCompletableFuture();
        state.add(future);
        future.whenCompleteAsync(new ExecutionCallbackAdapter<>(new LongExecutionCallback(probe, startNanos)));
    }

    @TimeStep(prob = -1)
    public void get(ThreadState state, Probe probe, @StartNanos long startNanos) {
        IAtomicLong counter = state.getRandomCounter();
        CompletableFuture<Long> future = counter.getAsync().toCompletableFuture();
        state.add(future);
        future.whenCompleteAsync(new ExecutionCallbackAdapter<>(new LongExecutionCallback(probe, startNanos)));
    }

    @AfterRun
    public void afterRun(ThreadState state) {
        totalCounter.addAndGet(state.increments);
    }

    public class ThreadState extends BaseThreadState {

        final List<CompletableFuture> batch = new LinkedList<>();
        long increments;

        void add(CompletableFuture<Long> future) {
            if (batchSize <= 0) {
                return;
            }

            batch.add(future);
            if (batch.size() == batchSize) {
                for (CompletableFuture batchFuture : batch) {
                    try {
                        batchFuture.get();
                    } catch (Exception e) {
                        throw rethrow(e);
                    }
                }
                batch.clear();
            }
        }

        IAtomicLong getRandomCounter() {
            int index = randomInt(counters.length);
            return counters[index];
        }
    }

    @Verify
    public void verify() {
        if (isClient(targetInstance)) {
            return;
        }

        final String serviceName = totalCounter.getServiceName();
        final long expected = totalCounter.get();

        // since the operations are asynchronous, we have no idea when they complete
        assertTrueEventually(new AssertTask() {
            @Override
            public void run() throws Exception {
                // hack to prevent overloading the system with get calls, else it is done many times a second
                sleepSeconds(10);

                long actual = 0;
                for (DistributedObject distributedObject : targetInstance.getDistributedObjects()) {
                    String key = distributedObject.getName();
                    if (serviceName.equals(distributedObject.getServiceName()) && key.startsWith(name)) {
                        actual += getAtomicLong(key).get();
                    }
                }

                assertEquals(expected, actual);
            }
        }, assertEventuallySeconds);
    }

    @Teardown
    public void teardown() {
        if (isMemberNode(targetInstance)) {
            for (IAtomicLong counter : counters) {
                counter.destroy();
            }
        }
        totalCounter.destroy();
     }

    private class LongExecutionCallback implements ExecutionCallback<Long> {
        private final Probe probe;
        private final long startNanos;

        LongExecutionCallback(Probe probe, long startNanos) {
            this.probe = probe;
            this.startNanos = startNanos;
        }

        @Override
        public void onResponse(Long aLong) {
            probe.done(startNanos);
        }

        @Override
        public void onFailure(Throwable throwable) {
            onResponse(startNanos);
            ExceptionReporter.report(testContext.getTestId(), throwable);
        }
    }
}
