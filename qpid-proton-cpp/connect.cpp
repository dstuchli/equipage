/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
 */

#include <proton/default_container.hpp>

#include <proton/connection.hpp>
#include <proton/connection_options.hpp>
#include <proton/container.hpp>
#include <proton/delivery.hpp>
#include <proton/link.hpp>
#include <proton/message.hpp>
#include <proton/messaging_handler.hpp>
#include <proton/tracker.hpp>

#include <iostream>
#include <string>

struct connect_handler : public proton::messaging_handler {
    std::string conn_url_;

    void on_container_start(proton::container& cont) override {
        cont.connect(conn_url_); //, opts);
    }

    void on_connection_open(proton::connection& conn) override {
        std::cout << "CONNECT: Connected to '" << conn_url_ << "'" << std::endl;
        conn.close();
    }
};

int main(int argc, char** argv) {
    connect_handler h;
    h.conn_url_ = argv[1];

    proton::container container(h);

    container.run();

    return 0;
}
