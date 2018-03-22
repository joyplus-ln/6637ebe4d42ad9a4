package com.zp.sudoku;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

public class Generator {
	public static char[] data = new char[81];
	private char[] tmp = new char[81];
	private char[] map = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
	private Solver solver;

	private Generator() {
		solver = new Solver();
	}

	private void randomFill() {
		int x;
		MWC.setSeed((int) System.currentTimeMillis());
		for (int i = 0; i < 81; i++) {
			data[i] = '.';
		}
		for (int i = 0; i < 9; i++) {
			do
				x = MWC.random() >> 9 & 15;
			while (x > 8);
			data[i * 9 + x] = map[i];
		}
		solver.load(data);
		solver.dfs(0);
	}

	private boolean[] hasTryed = new boolean[81];

	private void digHole() {
		char ch;
		int i, j, k, m;
		for (i = 0; i < 81; i++)
			hasTryed[i] = false;
		boolean flag;
		for (i = 0; i < 81; i++) {
			do
				m = MWC.random() >> 8 & 127;
			while (m >= 81 || hasTryed[m]);
			hasTryed[m] = true;
			ch = data[m];
			flag = false;
			for (j = 0; j < 9; j++)
				if (ch != map[j]) {
					for (k = 0; k < 81; k++)
						tmp[k] = data[k];
					tmp[m] = map[j];
					solver.load(tmp);
					solver.dfs(0);
					if (solver.hasResult()) {
						flag = true;
						break;
					}
				}
			if (!flag)
				data[m] = '.';
			else
				data[m] = ch;
		}
	}

	private static Generator generator = null;

	public static String getSudoke() {
		if (generator == null) {
			generator = new Generator();
		}
		generator.randomFill();
		generator.digHole();
		return String.valueOf(data);
	}
	
	public static char[] getData(){
		if (generator == null) {
			generator = new Generator();
		}
		generator.randomFill();
		generator.digHole();
		return data;
	}

	public static void main(String[] args) {
		// Generator g = new Generator();
		// g.randomFill();
		// System.out.println(g.data);
		// g.digHole();
		// System.out.println(g.data);
		//
		// Solver solver = new Solver();
		// solver.load(g.data);
		// solver.dfs(0);
		// if (solver.hasResult()) {
		// System.out.println(g.data);
		// }
		long startTime = System.nanoTime();
		BufferedWriter bw = null;
		File file = new File("D:/workspace/sudoku/assets/puzzles/a.txt");
		try {
			bw = new BufferedWriter(new FileWriter(file));

			for (int i = 0; i < 1; i++) {
				System.out.println(Generator.getSudoke());
				bw.write(Generator.getSudoke() + "\r\n");
			}
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} finally {
			try {
				if(bw != null)
					bw.close();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		float deltaTime = (System.nanoTime() - startTime) / 1000000000.0f;
		System.out.println(deltaTime);
	}
}
