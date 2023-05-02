module Demo {
    sequence<byte> ImageData;

    struct Image{
        int width;
        int height;
        ImageData data;
    };

    interface Operations {
        void simpleOperation();
        string operationWithArgs(string arg1, int arg2);
        Image processData(Image data);
    };
};
