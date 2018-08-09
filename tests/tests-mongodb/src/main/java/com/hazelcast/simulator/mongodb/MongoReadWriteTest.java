/*
 * Copyright (c) 2008-2018, Hazelcast, Inc. All Rights Reserved.
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
package com.hazelcast.simulator.mongodb;

import com.hazelcast.simulator.test.BaseThreadState;
import com.hazelcast.simulator.test.annotations.Prepare;
import com.hazelcast.simulator.test.annotations.Setup;
import com.hazelcast.simulator.test.annotations.Teardown;
import com.hazelcast.simulator.test.annotations.TimeStep;
import com.mongodb.BasicDBObject;
import com.mongodb.client.MongoCollection;
import com.mongodb.client.MongoDatabase;
import com.mongodb.client.model.Filters;
import java.util.Random;
import org.bson.Document;

public class MongoReadWriteTest extends MongoDBTest {

    public int size = 1000;
    public int propertySize = 100;
    public String databaseName = "test";
    public String collectionName = "readWriteTest";

    private MongoCollection<Document> col;

    @Setup
    public void setUp() {
        if (size <= 0) {
            throw new IllegalStateException("size must be larger than 0");
        }

        MongoDatabase database = client.getDatabase(databaseName);
        col = database.getCollection(collectionName);
    }

    @Prepare
    public void prepare() {
        Random random = new Random();
        MongoDatabase database = client.getDatabase(databaseName);
        MongoCollection<Document> col = database.getCollection(collectionName);
        for (int i = 0; i < size; i++) {
            col.insertOne(Document.parse("{ \"_id\": " + i + ", \"a\": { \"b\": " + random.nextInt(propertySize) + ", \"name\": \"whatever\" } }"));
        }
    }

    @TimeStep(prob = 0.1)
    public void put(ThreadState state) {
        int randomId = state.randomId();
        int randomValue = state.randomValue();
        col.replaceOne(Filters.eq("_id", randomId), Document.parse("{ \"_id\": " + randomId + ", \"a\": { \"b\": " + randomValue + ", \"name\": \"whatever\" } }"));
    }

    @TimeStep(prob = -1)
    public Object get(ThreadState state) {
        BasicDBObject query = new BasicDBObject("a.b", state.randomValue());
        return col.find(query);
    }

    public class ThreadState extends BaseThreadState {

        private int randomId() {
            return randomInt(size);
        }

        private int randomValue() {
            return randomInt(propertySize);
        }
    }

    @Teardown
    public void tearDown() {
        client.close();
    }
}
