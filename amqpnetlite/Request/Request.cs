﻿/*
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

using System;
using System.Threading;
using Amqp;
using Amqp.Framing;

namespace Request
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Usage: Request <connection-url> <address> <message-body>");
                Environment.Exit(1);
            }

            string connUrl = args[0];
            string address = args[1];
            string messageBody = args[2];

            Connection conn = new Connection(new Address(connUrl));

            try
            {
                Console.WriteLine("REQUEST: Connected to '{0}'", connUrl);

                Session session = new Session(conn);
                SenderLink sender = new SenderLink(session, "send-1", address);

                Console.WriteLine("REQUEST: Created sender for target address '{0}'", address);

                string responseAddress = null;
                ManualResetEvent done = new ManualResetEvent(false);
                Source source = new Source() { Dynamic = true };

                OnAttached onReceiverAttached = (link, attach) => {
                    responseAddress = ((Source) attach.Source).Address;
                    done.Set();
                };

                ReceiverLink receiver = new ReceiverLink(session, "receive-1", source, onReceiverAttached);
                done.WaitOne();

                Message request = new Message(messageBody);
                request.Properties = new Properties() { ReplyTo = responseAddress };

                sender.Send(request);

                Console.WriteLine("REQUEST: Sent request '{0}'", messageBody);

                Message response = receiver.Receive();

                Console.WriteLine("REQUEST: Received response '{0}'", response.Body);

                done.WaitOne(1000);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
