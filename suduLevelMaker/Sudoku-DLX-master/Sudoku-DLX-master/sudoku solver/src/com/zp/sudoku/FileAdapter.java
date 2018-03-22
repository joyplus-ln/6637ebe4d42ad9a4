package com.zp.sudoku;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;

public class FileAdapter {
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
	
	public void excute(){
		String line = "";
		try {
			while(true){
				line = bf.readLine();
				if(line == null)
					break;
				bw.write("\"" + line + "\"," + "\r\n");
			}
		} catch (Exception e) {
			// TODO: handle exception
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
		FileAdapter fileAdapter = new FileAdapter();
		fileAdapter.openFile("D:/sudoku puzzle/new_level8.txt");
		fileAdapter.setOutputFile("D:/sudoku puzzle/new_level8_1.txt");
		fileAdapter.excute();
	}
}
