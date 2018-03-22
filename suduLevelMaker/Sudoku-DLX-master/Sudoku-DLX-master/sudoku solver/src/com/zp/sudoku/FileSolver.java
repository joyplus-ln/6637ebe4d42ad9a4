package com.zp.sudoku;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;

public class FileSolver {
	private BufferedReader bf;
	private BufferedWriter bw;
	
	public void openFile(String fileName){
		File file = new File(fileName);
		try {
			bf = new BufferedReader(new FileReader(file));
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void setOutputFile(String fileName){
		File file = new File(fileName);
		try {
			bw = new BufferedWriter(new FileWriter(file));
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void solve(){
		Solver solver = new Solver();
		String line = "";
		int count;
		try {
			while(true){
				line = bf.readLine();
				if(line == null)
					break;
				if(line.startsWith("#") || line.isEmpty())
					continue;
				//String[] parts = line.split("\\|");
				count = 0;
				for(int i = 0; i < 81; i++){
					if(line.charAt(i) != '.')
						count ++;
				}
				solver.load(line);
				solver.dfs(0);
				System.out.println(line + "|" + solver.getResult() + "|" + count);
				bw.write(solver.getResult() + "|" + count + "\r\n");
			}
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} finally {
			try {
				bf.close();
				bw.close();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	public static void main(String[] args){
		FileSolver file = new FileSolver();
		file.openFile("D:/sudoku puzzle/new_level5.txt");
		file.setOutputFile("D:/sudoku puzzle/new_level5_res.txt");
		file.solve();
	}
}
