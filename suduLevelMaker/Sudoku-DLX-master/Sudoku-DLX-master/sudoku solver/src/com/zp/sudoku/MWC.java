package com.zp.sudoku;

public class MWC {
	private static int zr;
	private static int wr;
	
	public static void setSeed(int seed) {
		zr ^= seed;
		wr += seed;
	}

	public static int random() {
		zr = 36969 * (zr & 65535) + (zr >> 16);
		wr = 18000 * (wr & 65535) + (wr >> 16);
		return zr ^ wr;
	}

}
