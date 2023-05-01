package org.example;

import com.zeroc.Ice.*;
import com.zeroc.Ice.Object;
public class Main {

    private ObjectPrx proxy;

    public Main(ObjectPrx proxy) {
        this.proxy = proxy;
    }

    public void simpleOperation() {
        proxy.ice_invoke("simpleOperation", OperationMode.Normal, new byte[0]);
    }

    public String operationWithArgs(String arg1, int arg2) {
        OutputStream out = new OutputStream(proxy.ice_getCommunicator());
        out.startEncapsulation();
        out.writeString(arg1);
        out.writeInt(arg2);
        out.endEncapsulation();

        byte[] outParams = out.finished();


        Object.Ice_invokeResult inParams = proxy.ice_invoke("operationWithArgs", OperationMode.Normal, outParams);

        InputStream in = new InputStream(proxy.ice_getCommunicator(), inParams.outParams);
        in.startEncapsulation();
        String returnValue = in.readString();
        in.endEncapsulation();

        return returnValue;
    }

    public byte[] processData(byte[] data) {
        OutputStream out = new OutputStream(proxy.ice_getCommunicator());
        out.startEncapsulation();
        out.writeByteSeq(data);
        out.endEncapsulation();

        byte[] outParams = out.finished();

        Object.Ice_invokeResult inParams = proxy.ice_invoke("processData", OperationMode.Normal, outParams);

        InputStream in = new InputStream(proxy.ice_getCommunicator(), inParams.outParams);
        in.startEncapsulation();
        int size = in.readAndCheckSeqSize(1);
        byte[] returnValue = in.readBlob(size);
        in.endEncapsulation();

        return returnValue;
    }


    public static void main(String[] args) {
        try (Communicator communicator = Util.initialize(args)) {
            ObjectPrx proxy = communicator.stringToProxy("Operations:default -h localhost -p 10000");
            Main client = new Main(proxy);

            client.simpleOperation();
            String result1 = client.operationWithArgs("hello", 123);
            byte[] result2 = client.processData("some data".getBytes());

            System.out.println(result1);
            System.out.println(new String(result2));

            communicator.waitForShutdown();
        }
    }
}