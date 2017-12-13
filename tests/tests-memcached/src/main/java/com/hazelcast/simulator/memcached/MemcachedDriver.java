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
package com.hazelcast.simulator.memcached;

import com.hazelcast.simulator.agent.workerprocess.WorkerParameters;
import com.hazelcast.simulator.vendors.VendorDriver;

import java.net.InetSocketAddress;
import java.util.ArrayList;
import java.util.List;

import static com.hazelcast.simulator.utils.FileUtils.fileAsText;
import static java.lang.String.format;

public class MemcachedDriver extends VendorDriver<MemcachedClientFactory> {

    private MemcachedClientFactory clientFactory;

    @Override
    public WorkerParameters loadWorkerParameters(String workerType, int agentIndex) {
        WorkerParameters params = new WorkerParameters()
                .setAll(properties)
                .set("WORKER_TYPE", workerType)
                .set("file:log4j.xml", loadLog4jConfig());

        if ("javaclient".equals(workerType)) {
            loadClientParameters(params);
        } else {
            throw new IllegalArgumentException(format("Unsupported workerType [%s]", workerType));
        }

        return params;
    }

    private void loadClientParameters(WorkerParameters params) {
        params.set("JVM_OPTIONS", get("CLIENT_ARGS", ""))
                .set("nodes", fileAsText("nodes.txt"))
                .set("file:worker.sh", loadWorkerScript("javaclient"));
    }

    @Override
    public MemcachedClientFactory getVendorInstance() {
        return clientFactory;
    }

    @Override
    public void startVendorInstance() throws Exception {
        String[] nodes = get("nodes").split(",");
        List<InetSocketAddress> addresses = new ArrayList<InetSocketAddress>();
        for (String node: nodes) {
            String[] addressParts = node.split(":");
            if (addressParts.length == 0 || addressParts.length > 2) {
                throw new IllegalArgumentException("Invalid node address. Example: localhost:11211");
            }

            int port = 11211; //default memcached port
            if (addressParts.length == 2) {
                port = Integer.parseInt(addressParts[1]);
            }
            addresses.add(new InetSocketAddress(addressParts[0], port));
        }

        clientFactory = new MemcachedClientFactory(addresses);
    }

    @Override
    public void close() {
        clientFactory.shutdown();
    }
}
