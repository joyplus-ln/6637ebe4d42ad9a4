package com.zp.sudoku;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Random;

public class LevelGenerator {
	public LevelGenerator() {
		// TODO Auto-generated constructor stub
	}
	private char[] res = new char[81];
	Solver solver = new Solver();
	Random random = new Random();
	
	private int[] hintsAtCloumn = new int[9];
	
	private static final int LowerBound = 0;
	private static final int Total = 24;
	private static final int Total_UperBound = 26;
	
	public boolean isInvalid = false;
	
	public void handle(char[] data){
		int i, j, k, r, sum;
		for(i = 0; i < 81; i++)
			res[i] = data[i];
		for(i = 0; i < 9; i++){
			hintsAtCloumn[i] = 0;
			for(j = 0; j < 9; j++){
				if(data[j*9 + i] != '.')
					hintsAtCloumn[i]++;
			}
		}
		solver.load(res);
		solver.dfs(0);
		sum = 0;
		for(i = 0; i < 9; i++){
			k = 0;
			for(j = 0; j < 9; j++){
				if(data[i * 9 + j] != '.'){
					k++;
				}
			}
			while(k < LowerBound) {
//				do r = MWC.random() >> 9 & 15; while(r > 8 || data[i * 9 + r] != '.');
				int minColumn = 0, minCells;
				minCells = 9;
				for(int m = 0; m < 9; m++){
					if(data[i * 9 + m] == '.' && hintsAtCloumn[m] < minCells){
						minCells = hintsAtCloumn[m];
						minColumn = m;
					}
				}
				data[i * 9 + minColumn] = res[i * 9 + minColumn];
				hintsAtCloumn[minColumn]++;
				k++;
			}
			sum += k;
		}
		if(sum > Total_UperBound){
			isInvalid = true;
			return;
		}
		isInvalid = false;
		k = random.nextInt(Total_UperBound - Total);
		for(; sum < Total + k; sum++){
			do r = MWC.random() >> 8 & 127; while(r > 80 || data[r] != '.');
			data[r] = res[r];
		}
		System.out.println("" + sum);
	}
	
	public static void main(String[] args){
		LevelGenerator lgGenerator = new LevelGenerator();
		char[] data;
		char[] temp = new char[81];
		data = Generator.getData();
		lgGenerator.handle(data);
		System.out.println(data);
		Solver solver = new Solver();
		
		BufferedWriter bw = null;
		File file = new File("D:/sudoku puzzle/new_level8.txt");
		try {
			bw = new BufferedWriter(new FileWriter(file));

			for (int i = 0; i < 500; i++) {
				data = Generator.getData();
				lgGenerator.handle(data);
				if(lgGenerator.isInvalid){
					i--;
					continue;
				}
				for(int j = 0; j < 81; j++){
					temp[j] = data[j];
				}
				
				solver.load(temp);
				solver.dfs(0);
				if(!solver.hasResult()){
					System.out.println(String.valueOf(temp) + " no result ");
					i--;
					continue;
				}
				System.out.println(data);
				bw.write(String.valueOf(data) + "\r\n");
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
	}
}
