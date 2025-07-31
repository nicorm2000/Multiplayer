using System.Collections.Generic;
using Network_Lib.BasicMessages;
using System.Linq;
using System.Net;
using System;

namespace Net
{
    public class ReflectionMessageHandler
    {
        private readonly Reflection reflection;

        public ReflectionMessageHandler(Reflection reflection)
        {
            this.reflection = reflection;
        }

        #region Message Handling
        /// <summary>
        /// Handles received reflection messages and routes them to appropriate processing methods.
        /// </summary>
        /// <param name="data">The raw message data.</param>
        /// <param name="ip">The endpoint from which the message was received.</param>
        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            try
            {
                //debugger?.Log($"Received {data.Length} bytes: {BitConverter.ToString(data)}");
                string debug = "OnReceivedReflectionMessage - ";
                //// Verify minimum length
                //if (data == null || data.Length < 4)
                //{
                //    debugger?.Log("ERROR: Message too short");
                //    return;
                //}
                MessageType messageType = MessageChecker.CheckMessageType(data);
                //debugger?.Log($"Received MessageType: {messageType}");
                //debugger?.Log($"\nRAW DATA RECEIVED ({data?.Length ?? 0} bytes): {BitConverter.ToString(data ?? new byte[0])}");
                debug += $"Message Type: {messageType}\n";

                switch (messageType)
                {
                    case MessageType.Ulong:
                        debug += "Processing Ulong message\n";
                        NetULongMessage netULongMessage = new NetULongMessage(data);
                        debug += $"Data: {netULongMessage.GetData()}, Route: {string.Join("->", netULongMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netULongMessage.GetMessageRoute(), netULongMessage.GetData());
                        break;

                    case MessageType.Uint:
                        debug += "Processing Uint message\n";
                        NetUIntMessage netUIntMessage = new NetUIntMessage(data);
                        debug += $"Data: {netUIntMessage.GetData()}, Route: {string.Join("->", netUIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netUIntMessage.GetMessageRoute(), netUIntMessage.GetData());
                        break;

                    case MessageType.Ushort:
                        debug += "Processing Ushort message\n";
                        NetUShortMessage netUShortMessage = new NetUShortMessage(data);
                        debug += $"Data: {netUShortMessage.GetData()}, Route: {string.Join("->", netUShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netUShortMessage.GetMessageRoute(), netUShortMessage.GetData());
                        break;

                    case MessageType.String:
                        debug += "Processing String message\n";
                        NetStringMessage netStringMessage = new NetStringMessage(data);
                        debug += $"Data: {netStringMessage.GetData()}, Route: {string.Join("->", netStringMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netStringMessage.GetMessageRoute(), netStringMessage.GetData());
                        break;

                    case MessageType.Short:
                        debug += "Processing Short message\n";
                        NetShortMessage netShortMessage = new NetShortMessage(data);
                        debug += $"Data: {netShortMessage.GetData()}, Route: {string.Join("->", netShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netShortMessage.GetMessageRoute(), netShortMessage.GetData());
                        break;

                    case MessageType.Sbyte:
                        debug += "Processing Sbyte message\n";
                        NetSByteMessage netSByteMessage = new NetSByteMessage(data);
                        debug += $"Data: {netSByteMessage.GetData()}, Route: {string.Join("->", netSByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netSByteMessage.GetMessageRoute(), netSByteMessage.GetData());
                        break;

                    case MessageType.Long:
                        debug += "Processing Long message\n";
                        NetLongMessage netLongMessage = new NetLongMessage(data);
                        debug += $"Data: {netLongMessage.GetData()}, Route: {string.Join("->", netLongMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netLongMessage.GetMessageRoute(), netLongMessage.GetData());
                        break;

                    case MessageType.Int:
                        debug += "Processing Int message\n";
                        NetIntMessage netIntMessage = new NetIntMessage(data);
                        debug += $"Data: {netIntMessage.GetData()}, Route: {string.Join("->", netIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netIntMessage.GetMessageRoute(), netIntMessage.GetData());
                        break;

                    case MessageType.Float:
                        debug += "Processing Float message\n";
                        NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                        debug += $"Data: {netFloatMessage.GetData()}, Route: {string.Join("->", netFloatMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());
                        break;

                    case MessageType.Double:
                        debug += "Processing Double message\n";
                        NetDoubleMessage netDoubleMessage = new NetDoubleMessage(data);
                        debug += $"Data: {netDoubleMessage.GetData()}, Route: {string.Join("->", netDoubleMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netDoubleMessage.GetMessageRoute(), netDoubleMessage.GetData());
                        break;

                    case MessageType.Decimal:
                        debug += "Processing Decimal message\n";
                        NetDecimalMessage netDecimalMessage = new NetDecimalMessage(data);
                        debug += $"Data: {netDecimalMessage.GetData()}, Route: {string.Join("->", netDecimalMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netDecimalMessage.GetMessageRoute(), netDecimalMessage.GetData());
                        break;

                    case MessageType.Char:
                        debug += "Processing Char message\n";
                        NetCharMessage netCharMessage = new NetCharMessage(data);
                        debug += $"Data: {netCharMessage.GetData()}, Route: {string.Join("->", netCharMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netCharMessage.GetMessageRoute(), netCharMessage.GetData());
                        break;

                    case MessageType.Byte:
                        debug += "Processing Byte message\n";
                        NetByteMessage netByteMessage = new NetByteMessage(data);
                        debug += $"Data: {netByteMessage.GetData()}, Route: {string.Join("->", netByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netByteMessage.GetMessageRoute(), netByteMessage.GetData());
                        break;

                    case MessageType.Bool:
                        debug += "Processing Bool message\n";
                        NetBoolMessage netBoolMessage = new NetBoolMessage(data);
                        debug += $"Data: {netBoolMessage.GetData()}, Route: {string.Join("->", netBoolMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netBoolMessage.GetMessageRoute(), netBoolMessage.GetData());
                        break;

                    case MessageType.Null:
                        debug += "Processing Null message\n";
                        NetNullMessage netNullMessage = new NetNullMessage(data);
                        debug += $"Data: {netNullMessage.GetData()}, Route: {string.Join("->", netNullMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMappingNullException(netNullMessage.GetMessageRoute(), netNullMessage.GetData());
                        break;

                    case MessageType.Empty:
                        debug += "Processing Empty message\n";
                        NetEmptyMessage netEmptyMessage = new NetEmptyMessage(data);
                        debug += $"Data: {netEmptyMessage.GetData()}, Route: {string.Join("->", netEmptyMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMappingEmpty(netEmptyMessage.GetMessageRoute(), netEmptyMessage.GetData());
                        break;

                    case MessageType.Method:
                        debug += "Processing Method message\n";
                        NetMethodMessage netMethodMessage = new NetMethodMessage(data);
                        (int, List<(string, string)>) methodData = netMethodMessage.GetData();
                        debug += $"Method: {methodData.Item1}, Args: {string.Join(", ", methodData.Item2)}, Route: {netMethodMessage.GetMessageRoute()[0].route}\n";
                        //debugger?.Log(debug);
                        reflection.reflectionCallInvoker.InvokeReflectionMethod(methodData.Item1, methodData.Item2, netMethodMessage.GetMessageRoute()[0].route);
                        break;

                    case MessageType.Remove:
                        debug += "Processing Remove message\n";
                        NetRemoveMessage removeMessage = new NetRemoveMessage(data);
                        debug += $"Data: {removeMessage.GetData()}, Route: {string.Join("->", removeMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(removeMessage.GetMessageRoute(), removeMessage.GetData());
                        break;

                    case MessageType.Enum:
                        debug += "Processing Enum message\n";
                        NetEnumMessage netEnumMessage = new NetEnumMessage(data);
                        debug += $"Enum: {netEnumMessage.GetData()}, Route: {string.Join("->", netEnumMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.VariableMapping(netEnumMessage.GetMessageRoute(), netEnumMessage.GetData());
                        break;

                    case MessageType.Event:
                        debug += "Processing Event message\n";
                        NetEventMessage netEventMessage = new NetEventMessage(data);
                        (int, List<(string, string)>) eventData = netEventMessage.GetData();
                        debug += $"Method: {eventData.Item1}, Args: {string.Join(", ", eventData.Item2)}, Route: {netEventMessage.GetMessageRoute()[0].route}\n";
                        List<RouteInfo> route = netEventMessage.GetMessageRoute();
                        //debugger?.Log(debug);
                        reflection.reflectionCallInvoker.InvokeCSharpEvent(eventData.Item1, eventData.Item2, route[0].route);
                        break;

                    case MessageType.TRS:
                        debug += "Processing TRS message\n";
                        NetTRSMessage netTRSMessage = new NetTRSMessage(data);
                        debug += $"Enum: {netTRSMessage.GetData()}, Route: {string.Join("->", netTRSMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //debugger?.Log(debug);
                        reflection.TRSMapping(netTRSMessage.GetMessageRoute(), netTRSMessage.GetData());
                        break;

                    default:
                        debug += $"Unhandled message type: {messageType}\n";
                        //debugger?.Log(debug);
                        break;
                }
            }
            catch (Exception ex)
            {
                //debugger?.Log($"ERROR Processing Message: {ex.Message}");
            }
        }
        #endregion
    }
}
