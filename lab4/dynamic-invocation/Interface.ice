module Demo {
    sequence<byte> ComplexData;

    interface Operations {
        void simpleOperation();
        string operationWithArgs(string arg1, int arg2);
        ComplexData processData(ComplexData data);
    };
};
