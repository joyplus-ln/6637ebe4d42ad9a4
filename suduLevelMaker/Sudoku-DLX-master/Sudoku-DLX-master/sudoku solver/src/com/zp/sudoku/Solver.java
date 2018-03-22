package com.zp.sudoku;

public class Solver {
	private static final int COLUMN_SIZE = 324; 

	private char[] oridata;
	
	private int[] L;
	private int[] R;
	private int[] U;
	private int[] D;
	private int[] C;
	private int[] S;
	private final int Head = 0;
	private boolean[] wasRemoved;
	
	private static final int[][] square = {
		{0, 0, 0, 1, 1, 1, 2, 2, 2},
		{0, 0, 0, 1, 1, 1, 2, 2, 2},
		{0, 0, 0, 1, 1, 1, 2, 2, 2},
		{3, 3, 3, 4, 4, 4, 5, 5, 5},
		{3, 3, 3, 4, 4, 4, 5, 5, 5},
		{3, 3, 3, 4, 4, 4, 5, 5, 5},
		{6, 6, 6, 7, 7, 7, 8, 8, 8},
		{6, 6, 6, 7, 7, 7, 8, 8, 8},
		{6, 6, 6, 7, 7, 7, 8, 8, 8}
	};
	
	public Solver(){
		int total = COLUMN_SIZE + 4 * 729 + 1;
		L = new int[total];
		R = new int[total];
		U = new int[total];
		D = new int[total];
		C = new int[total];
		S = new int[325];
		wasRemoved = new boolean[325];
	}
	
	public void load(String puzzle){
		this.oridata = puzzle.toCharArray();
		initDLX();
	}
	
	public void load(char[] oridata){
		this.oridata = oridata;
		initDLX();
	}
	
	private interface Method{
		public int getColumn(int i, int j, int k);
	}
	
	private final Method[] condition = {
			new Method(){
				@Override
				public int getColumn(int i, int j, int k) {
					return 1 + i * 9 + k;
				}
			},
			new Method(){
				@Override
				public int getColumn(int i, int j, int k) {
					return 82 + j * 9 + k;
				}
			},
			new Method(){
				@Override
				public int getColumn(int i, int j, int k) {
					return 163 + square[i][j] * 9 + k;
				}
			},
			new Method(){
				@Override
				public int getColumn(int i, int j, int k) {
					return 244 + i * 9 + j;
				}
			}
			
	};
	
	private void initDLX(){
		int i, j, k, n;
		for(i = 0; i <= 324; i++){
			L[i] = i - 1;
			R[i] = i + 1;
			U[i] = i;
			D[i] = i;
		}
		L[0] = 324;
		R[324] = 0;
		for(i = 0; i < 325; i++){
			S[i] = 0;
			wasRemoved[i] = false;
		}

		int cur = 325;
		for(i = 0; i < 9; i++)
			for(j = 0; j < 9; j++)
				for(k = 0; k < 9; k++){
					for(n = 0; n < 4; n++){
						R[cur] = cur + 1;
						L[cur] = cur - 1;
						C[cur] = condition[n].getColumn(i, j, k);
						U[cur] = U[C[cur]];
						D[cur] = C[cur];
						D[U[cur]] = cur;
						U[C[cur]] = cur;
						S[C[cur]] ++;
						cur ++;
					}
					R[cur - 1] = cur - 4;
					L[cur - 4] = cur - 1;
				}
		
		
		for(i = 0; i < 9; i++)
			for(j = 0; j < 9; j++){
				char ch = (char)oridata[i * 9 + j];
				if(ch != '.'){
					k = ch - '1';
					for(n = 0; n < 4; n++){
						int column = condition[n].getColumn(i, j, k);
						if(!wasRemoved[column]){
							remove(column);
							wasRemoved[column] = true;
						}
					}
				}
			}
	}
	
	private void remove(int c){
		L[R[c]] = L[c];
		R[L[c]] = R[c];
		for(int i = D[c]; i != c; i = D[i]){
			for(int j = R[i]; j != i; j = R[j]){
				U[D[j]] = U[j];
				D[U[j]] = D[j];
				-- S[C[j]];
			}
		}
	}
	
	private void resume(int c){
		for(int i = U[c]; i != c; i = U[i]){
			for(int j = L[i]; j != i; j = L[j]){
				++ S[C[j]];
				D[U[j]] = j;
				U[D[j]] = j;
			}
		}
		R[L[c]] = c;
		L[R[c]] = c;
	}
	
	public boolean dfs(int k){
		if(k > 325) return true;
		int s = Integer.MAX_VALUE, c = 0;
		for(int t = R[Head]; t != Head; t = R[t]){
			if(S[t] < s){
				s = S[t];
				c = t;
			}
		}
		if(c == 0) return true;
		remove(c);
		for(int i = D[c]; i != c; i = D[i]){
			int t = (i - 325) / 4;
			oridata[t / 9] = (char)(t % 9 + '1');
			for(int j = R[i]; j != i; j = R[j]){
				remove(C[j]);
			}
			if(dfs(k + 1))
				return true;
			for(int j = L[i]; j != i; j = L[j]){
				resume(C[j]);
			}
		}
		resume(c);
		return false;
	}
	
	public boolean hasResult(){
		return Head == R[Head];
	}
	
	public String getResult(){
		return String.valueOf(oridata);
	}
	
	public static void main(String[] args){
		Solver solver = new Solver();
		solver.load("...............8.4....3..5...3..1.........4.....7..3.9.47.62..3.56.....2.32..49.8");
//		solver.load("678621349614937852932854617165783294429516783387249165256398471791462538843175926");
//		solver.load("6..62..49.1.9.785.9....4.171...8.2..4...1...3..7.4...525.3....1.914.2.3.84..75...");
		solver.dfs(0);
		if(solver.hasResult())
			System.out.println(solver.getResult());
		else {
			System.out.println("无解");
		}
//		solver.load(".....15931794.....8.....1...8.2.3.45..2...7..76.5.4.8...4.....9.....62585183.....");
//		System.out.println(solver.getResult());
	}
}
