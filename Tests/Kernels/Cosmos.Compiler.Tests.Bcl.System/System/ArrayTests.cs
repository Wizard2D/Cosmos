﻿using System;
using Cosmos.Debug.Kernel;
using Cosmos.TestRunner;

namespace Cosmos.Compiler.Tests.Bcl.System
{
    class ArrayTests
    {
        public static unsafe void Execute()
        {
            byte[] xEmptyByteArray = Array.Empty<byte>();
            object[] xEmptyObjectArray = Array.Empty<object>();

            Assert.IsTrue(xEmptyByteArray.Length == 0, "Array.Empty<byte> should return an empty array!");
            Assert.IsTrue(xEmptyObjectArray.Length == 0, "Array.Empty<object> should return an empty array!");

            byte[] xByteResult = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] xByteExpectedResult = { 1, 2, 3, 4, 5, 6, 7, 1 };
            byte[] xByteSource = { 1 };

            Assert.IsTrue(xByteExpectedResult.Length == 8, "Array length is stored correctly");
            Assert.IsTrue(xByteResult.GetLowerBound(0) == 0, "Array.GetLowerBound works");
            //xByteResult.SetValue(1, 0);
            Assert.IsTrue((int)xByteResult.GetValue(0) == 1, "Array.GetValue works for first element");
            Assert.IsTrue((int)xByteResult.GetValue(1) == 2, "Array.GetValue works for element in middle");
            Assert.IsTrue((int)xByteResult.GetValue(7) == 8, "Array.GetValue works at end");

            Array.Copy(xByteSource, 0, xByteResult, 7, 1);

            Assert.IsTrue((xByteResult[7] == xByteExpectedResult[7]), "Array.Copy doesn't work: xResult[7] =  " + (uint)xByteResult[7] + " != " + (uint)xByteExpectedResult[7]);
            Array.Clear(xByteResult, 0, xByteResult.Length);
            for (int i = 0; i < 8; i++)
            {
                Assert.IsTrue(xByteResult[i] == 0, "Array.Clear works");
            }
            xByteResult[1] = 1;
            Assert.IsTrue(xByteResult[1] == 1, "Array.Clear does not break the array");

            // Single[] Test
            float[] xSingleResult = { 1.25f, 2.50f, 3.51f, 4.31f, 9.28f, 18.56f };
            float[] xSingleExpectedResult = { 1.25f, 2.598f, 5.39f, 4.31f, 9.28f, 18.56f };
            float[] xSingleSource = { 0.49382f, 1.59034f, 2.598f, 5.39f, 7.48392f, 4.2839f };

            xSingleResult[1] = xSingleSource[2];
            xSingleResult[2] = xSingleSource[3];

            Assert.IsTrue(((xSingleResult[1] + xSingleResult[2]) == (xSingleExpectedResult[1] + xSingleExpectedResult[2])), "Assinging values to single array elements doesn't work: xResult[1] =  " + (uint)xSingleResult[1] + " != " + (uint)xSingleExpectedResult[1] + " and xResult[2] =  " + (uint)xSingleResult[2] + " != " + (uint)xSingleExpectedResult[2]);

            // Double[] Test
            double[] xDoubleResult = { 0.384, 1.5823, 2.5894, 2.9328539, 3.9201, 4.295 };
            double[] xDoubleExpectedResult = { 0.384, 1.5823, 2.5894, 95.32815, 3.9201, 4.295 };
            double[] xDoubleSource = { 95.32815 };

            xDoubleResult[3] = xDoubleSource[0];

            Assert.IsTrue(xDoubleResult[3] == xDoubleExpectedResult[3], "Assinging values to double array elements doesn't work: xResult[1] =  " + (uint)xDoubleResult[3] + " != " + (uint)xDoubleExpectedResult[3]);

            //Test array indexes
            int y = 0;
            int[] x = new int[5] { 1, 2, 3, 4, 5 };
            bool error = false;
            try
            {
                y = x[1];
                y = x[7];
            }
            catch (IndexOutOfRangeException)
            {
                error = true;
            }
            Assert.IsTrue(error && y == 2, "Index out of range exception works correctly for too large positions.");
            error = false;
            try
            {
                y = x[-1];
            }
            catch (IndexOutOfRangeException)
            {
                error = true;
            }
            Assert.IsTrue(error && y == 2, "Index out of range exception works correctly for too small positions.");

            fixed (int* val = x)
            {
                Assert.AreEqual(1, val[0], "Accessing values using pointer works at offset 0");
                Assert.AreEqual(2, val[1], "Accessing values using pointer works at offset 1");
                Assert.AreEqual(4, val[3], "Accessing values using pointer works at offset 3");
                Assert.AreEqual(5, val[4], "Accessing values using pointer works at offset 4");
            }
            int[] arr = new int[] { 0, 1, 2, 3, 4 };

            arr[2] = 6;

            // Try reading the values from the array via a pointer
            fixed (int* val = arr)
            {
                Assert.AreEqual(0, val[0], "Accessing values using pointer works at offset 0");
                Assert.AreEqual(1, val[1], "Accessing values using pointer works at offset 1");
                Assert.AreEqual(3, val[3], "Accessing values using pointer works at offset 3");
                Assert.AreEqual(4, val[4], "Accessing values using pointer works at offset 4");
            }


            char[] charArray = new char[] { 'A', 'a', 'Z', 'z', 'l' };
            charArray[2] = 'k';
            fixed (char* val = charArray)
            {
                Assert.AreEqual('A', val[0], "Accessing values using pointer works at offset 0");
                Assert.AreEqual('a', val[1], "Accessing values using pointer works at offset 1");
                Assert.AreEqual('z', val[3], "Accessing values using pointer works at offset 3");
                Assert.AreEqual('l', val[4], "Accessing values using pointer works at offset 4");
            }

            string[] stringArray = new string[] { "ABC", "BAB", "TAT", "A", "", "LA" };
            Array.Resize(ref stringArray, 3);
            Assert.AreEqual(new string[] { "ABC", "BAB", "TAT" }, stringArray, "Array.Resize works");

            stringArray = new string[10];
            stringArray[0] += "asd";
            Assert.AreEqual(stringArray[0], "asd", "Adding directly to array works");
        }
    }
}
