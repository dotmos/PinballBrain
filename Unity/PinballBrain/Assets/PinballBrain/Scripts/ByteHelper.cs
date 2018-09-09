using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ByteHelper {
    public static byte[] ToBytes(int number) {
        return BitConverter.GetBytes(number);
    }

    public static byte[] ToBytes(short number) {
        return BitConverter.GetBytes(number);
    }

    public static short ToShort(byte[] bytes) {
        return ToShort(bytes, 0);
    }

    public static short ToShort(byte[] bytes, int startIndex) {
        return BitConverter.ToInt16(bytes, startIndex);
    }

    public static int ToInt(byte[] bytes) {
        return ToInt(bytes, 0);
    }

    public static int ToInt(byte[] bytes, int startIndex) {
        return BitConverter.ToInt32(bytes, startIndex);
    }
}
