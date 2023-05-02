package org.example;

import java.util.Arrays;

public class Image {
    public int width;
    public int height;
    public byte[] data;

    public Image(int width, int height, byte[] data) {
        this.width = width;
        this.height = height;
        this.data = data;
    }

    @Override
    public String toString() {
        return "Image{" +
                "width=" + width +
                ", height=" + height +
                ", data=" + Arrays.toString(data) +
                '}';
    }
}
