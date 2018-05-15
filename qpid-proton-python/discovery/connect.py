#!/usr/bin/env python
#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
#

from __future__ import print_function

import sys

from proton.handlers import MessagingHandler
from proton.reactor import Container

class ConnectHandler(MessagingHandler):
    def on_start(self, event):
        # Looks for:
        #
        #  1. $PWD/connect.json
        #  2. $HOME/.config/qpid-proton/connect.json
        #  3. /etc/qpid-proton/connect.json
        #
        # Note: I intend no linkage of the Python file name and the
        # JSON one here.  They just happen to coincide.
        event.container.connect_discover()

    def on_connection_opened(self, event):
        print("CONNECT: Connected to '{0}'".format(event.connection))
        event.connection.close()

def main():
    handler = ConnectHandler()
    container = Container(handler)
    container.run()

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        pass