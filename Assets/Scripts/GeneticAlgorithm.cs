using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {

	public List<int[]> population;
	
	private int popSize;
	private int dnaSize;
	private int numGen;
	private bool[,] Board;
	private int myPositionX;
	private int myPositionY;
	private int enemyPositionX;
	private int enemyPositionY;
	private int boardSizeX;
	private int boardSizeY;
	private int losingPenalty;
	private int winningReward;
	private float mutationRate;
	private System.Random rand;

	private bool Voronoi = false;


	public GeneticAlgorithm(int pop_size, int dna_size, int num_gen, bool[,] board, int my_position_x, int my_position_y, int enemy_position_x, int enemy_position_y, 
		int board_size_x, int board_size_y, int winning_reward, int losing_penalty, int seed, bool voronoi, float mutation_rate = 0.01f) {
		
		Board = board;
		popSize = pop_size;
		dnaSize = dna_size;
		numGen = num_gen;
		losingPenalty = losing_penalty;
		winningReward = winning_reward;
		myPositionX = my_position_x;
		myPositionY = my_position_y;
		enemyPositionX = enemy_position_x;
		enemyPositionY = enemy_position_y;
		boardSizeX = board_size_x;
		boardSizeY = board_size_y;
		mutationRate = mutation_rate;
		population = new List<int[]>();
		rand = new System.Random(seed);
		Voronoi = voronoi;
		
		int[] aux;
		// Debug.Log("Criando população:");
		// Debug.Log("Geração 0:");
		for(int i = 0; i < popSize; i++) {
			// Debug.Log("ind " + i);
			aux = new int[dnaSize+1];
			for(int j = 0; j < dnaSize; j++) {
				aux[j] = GetRandomInteger(4);
				// Debug.Log("gene " + j + " " + aux[j]);
			}
			aux[dnaSize] = calcFitness(aux, Board);
			// Debug.Log("gene " + dnaSize + " " + aux[dnaSize]);
			population.Add(aux);
		}
		// //Debug.Log("PORRA");
		// for(int i = 0; i < population.Count; i++){
		// 	for(int j = 0; j < population[i].Length; j++){
		// 		//Debug.Log(population[i][j]);
		// 	}
		// }
	}
	
	//roda o algoritmo inteiro
	public List<int[]> RunAlgorithm() {
		// Debug.Log("Rodando o algoritmo:");
		for(int i = 0; i < numGen; i++) {
			population = NewGen(population);
			// Debug.Log("Geração " + (i + 1));
			// for(int j = 0; j < population.Count; j++){
			// 	Debug.Log("ind "+ j);
			// 	for(int k = 0; k < population[j].Length; k++){
			// 		Debug.Log("gene " + k + " " + population[j][k]);
			// 	}
			// }
			
		}
		//retorna uma Lista com os dois melhores vetores de genes
		return population.GetRange(0, popSize);
	}
	

	public List<int[]> NewGen(List<int[]> pop)	{
		
		for(int i = 0; i < (popSize / 2); i++) {
			pop.AddRange(Crossover(pop[i], pop[popSize - 1 - i]));
		}
		pop.Sort((x,y)=> - (x[dnaSize]).CompareTo(y[dnaSize]));
		return (pop.GetRange(0, popSize));
	}
	
	public List<int[]> Crossover(int[] cr1, int[] cr2, int divisor = 2) {
		int[] creature1 = new int[dnaSize + 1];
		int[] creature2 = new int[dnaSize + 1];
		for(int i = 0; i < dnaSize; i++) {
			creature1[i] = cr1[i];
			creature2[i] = cr2[i];
		}
		// Debug.Log("before");
		// for(int i = 0; i < creature1.Length; i++){
		// 	Debug.Log("cr 1 " + creature1[i]);
		// }
		// for(int i = 0; i < creature2.Length; i++){
		// 	Debug.Log("cr 2 " + creature2[i]);
		// }
		int aux = 0;
		for(int i = divisor; i < dnaSize; i++) {
			aux = creature1[i];
			creature1[i] = creature2[i];
			creature2[i] = aux;
		}
		/*Debug.Log("after");
		for(int i = 0; i < creature1.Length; i++){
			Debug.Log("cr 1 " + creature1[i]);
		}
		for(int i = 0; i < creature2.Length; i++){
			Debug.Log("cr 2 " + creature2[i]);
		}*/
		
		
		creature1 = Mutation(creature1, mutationRate);
		creature2 = Mutation(creature2, mutationRate);
		
		
		creature1[dnaSize] = calcFitness(creature1, Board);
		creature2[dnaSize] = calcFitness(creature2, Board);
		List<int[]> pop = new List<int[]>();
		// Debug.Log("creatures after crossover");
		// for(int i = 0; i < creature1.Length; i++){
		// 	Debug.Log("cr 1 " + creature1[i]);
		// }
		// for(int i = 0; i < creature2.Length; i++){
		// 	Debug.Log("cr 2 " + creature2[i]);
		// }
		// Debug.Log("pop after crossover before add");
		// for(int j = 0; j < population.Count; j++){
		// 		Debug.Log("ind "+ j);
		// 		for(int k = 0; k < population[j].Length; k++){
		// 			Debug.Log("gene " + k + " " + population[j][k]);
		// 		}
		// 	}
		pop.Add(creature1);
		pop.Add(creature2);
		// Debug.Log("pop after crossover after add");
		// for(int j = 0; j < population.Count; j++){
		// 		Debug.Log("ind "+ j);
		// 		for(int k = 0; k < population[j].Length; k++){
		// 			Debug.Log("gene " + k + " " + population[j][k]);
		// 		}
		// 	}
		
		return pop;
	}
	
	public int[] Mutation(int[] creature, float mutation_rate) {
		if( GetRandomInteger(100) < mutation_rate*100) {
			
			creature[GetRandomInteger(dnaSize)] = GetRandomInteger(4);
		}
		return creature;
	}
	
	public int GetRandomInteger(int maximum)	{	
            return (rand.Next()%maximum);
			//return UnityEngine.Random.Range(1, maximum);
            // or call your class level instance of Random
    }
	
	public int calcFitness(int[] gene, bool[,] board, float random_multiplier = 1f) {
		int fitness = 0;
		if(lost(gene, dnaSize, myPositionX, myPositionY, board)) {
			// Debug.Log("fitness= "+ fitness);
			// Debug.Log("entered lost");
			fitness += losingPenalty;
			// Debug.Log("fitness= "+ fitness);
		} else {
			if(Voronoi) {
				if(Math.Abs(myPositionX - enemyPositionX) + Math.Abs(myPositionY - enemyPositionY) - dnaSize <= 2) {
					fitness += (losingPenalty / 2);
				}
				fitness += VoronoiDiagrams(gene, board, myPositionX, myPositionY, enemyPositionX, enemyPositionY);
			}else{
				fitness += normalFitness(gene, board);
			}
		}
		// //if(won(dnaSize, positionX, positionY, boardSizeX, boardSizeY)	
		// // fitness += Math.Abs(myPositionX-enemyPositionX);
		// if(myPositionX >= enemyPositionX) {
		// 	fitness -= (myPositionX - enemyPositionX);
		// 	} else {
		// 	fitness += (myPositionX - enemyPositionX);
		// }
		// // Debug.Log("fitness= "+ fitness);
		// if(myPositionY >= enemyPositionY) {
		// 	fitness -= (myPositionY - enemyPositionY);
		// } else {
		// 	fitness += (myPositionY - enemyPositionY);
		// }
		// // Debug.Log("fitness= "+ fitness);
		// fitness += GetRandomInteger((int) Math.Floor((boardSizeX + boardSizeY) * random_multiplier));
		// // Debug.Log("fitness= "+ fitness);

		return fitness;
	}

	public int normalFitness(int[] gene, bool[,] board, float random_multiplier = 1f) {
		int fitness = 0;
		if(myPositionX >= enemyPositionX) {
			fitness -= (myPositionX - enemyPositionX);
			} else {
			fitness += (myPositionX - enemyPositionX);
		}
		// Debug.Log("fitness= "+ fitness);
		if(myPositionY >= enemyPositionY) {
			fitness -= (myPositionY - enemyPositionY);
		} else {
			fitness += (myPositionY - enemyPositionY);
		}
		// Debug.Log("fitness= "+ fitness);
		fitness += GetRandomInteger((int) Math.Floor((boardSizeX + boardSizeY) * random_multiplier));
		return fitness;
	}

	public int VoronoiDiagrams(int[] gene, bool [,] board, int posX, int posY, int enemyPositionX, int enemyPositionY) {
		int myDomination = 1;
		int enemyDomination = 1;
		List<int[]> myQ = new List<int[]>();
		List<int[]> enemyQ = new List<int[]>();
		int position_x = posX;
		int position_y = posY;
		int ene_position_x = enemyPositionX;
		int ene_position_y = enemyPositionY;
		int[] pos_aux = new int[2];

		
		bool[,] auxBoard = new bool[boardSizeX, boardSizeY];
		for(int i = 0; i < boardSizeX; i++){
			for(int j = 0; j < boardSizeY; j++){
				auxBoard[i,j] = board[i,j];
			}
		}
		auxBoard[posX, posY] = true;
		auxBoard[enemyPositionX, enemyPositionY] = true;
		for(int i = 0; i < dnaSize; i++){
			switch(gene[i]) {
				case 2:
					if((position_x - 1) < 0 || auxBoard[position_x - 1,position_y] == true) {
					}else {
						auxBoard[position_x,position_y] = true;
						position_x--;
					}
					break;
				case 3:
					if((position_y + 1) >= boardSizeY || auxBoard[position_x,position_y + 1] == true) {
					} else {
						auxBoard[position_x,position_y] = true;
						position_y++;
					}
					break;
				case 0:
					if((position_x + 1) >= boardSizeX || auxBoard[position_x + 1,position_y] == true) {
					} else {
						auxBoard[position_x,position_y] = true;
						position_x++;
					}
					break;
				case 1:
					if((position_y - 1) < 0 || auxBoard[position_x,position_y - 1] == true) {
					} else {
						auxBoard[position_x,position_y] = true;
						position_y--;
					}
					break;
				default:
					Console.WriteLine("Error: impossible path");
					break;
			}
		}

		// auxBoard[position_x,position_y] = true;
		// string auxs = "";
		// for(int i = 0; i < boardSizeX; i++){
		// 	for(int j = 0; j < boardSizeY; j++){
		// 		if(auxBoard[i,j]) {
		// 			auxs += "1";
		// 		} else {
		// 			auxs += "0";
		// 		}
		// 	}
		// 	Console.WriteLine(auxs);
		// 	auxs = "";
		// }
		// Console.WriteLine("___________________________________");
		List<int[]> updateBoard1 = new List<int[]>();
		List<int[]> updateBoard2 = new List<int[]>();
		//updateBoard.Clear();
		pos_aux[0] = position_x;
		pos_aux[1] = position_y;
		// Console.WriteLine(pos_aux[0] + " " + pos_aux[1]);
		myQ.Add(pos_aux);
		pos_aux = new int[2];

								// Console.WriteLine("___________________________________");
		auxBoard[position_x, position_y] = true;
		pos_aux[0] = ene_position_x;
		pos_aux[1] = ene_position_y;
		enemyQ.Add(pos_aux);
		auxBoard[ene_position_x, ene_position_y] = true;

		int myStart = 0;
		int enemyStart = 0;
		int aux = myQ.Count;
		string auxs = "";
		int[] tmp = new int[2];

		while(myQ.Count > myStart || enemyQ.Count > enemyStart) {
			aux = myQ.Count;
			for(int i = myStart; i < aux; i++) {
				tmp = new int[2];
				tmp = myQ[i];
				// Console.WriteLine("valor na queue: " + myQ[i][0] + " " + myQ[i][1]);
				if(tmp[0] + 1 < boardSizeX && auxBoard[tmp[0] + 1, tmp[1]] == false && notInserted(tmp[0] + 1, tmp[1], updateBoard1)) {
					 // Console.WriteLine("Entrou caso 1");
					pos_aux = new int[2];
					pos_aux[0] = tmp[0] + 1;
					pos_aux[1] = tmp[1];
					updateBoard1.Add(pos_aux);
					myQ.Add(pos_aux);
					myDomination++;
				}
				if(tmp[0] - 1 >= 0 && auxBoard[tmp[0] - 1, tmp[1]] == false && notInserted(tmp[0] - 1, tmp[1], updateBoard1)) {
					 // Console.WriteLine("Entrou caso 2");
					pos_aux = new int[2];
					pos_aux[0] = tmp[0] - 1;
					pos_aux[1] = tmp[1];
					updateBoard1.Add(pos_aux);
					myQ.Add(pos_aux);
					myDomination++;
				}
				if(tmp[1] + 1 < boardSizeY && auxBoard[tmp[0], tmp[1] + 1] == false && notInserted(tmp[0] , tmp[1] + 1, updateBoard1)) {
					 // Console.WriteLine("Entrou caso 3");
					pos_aux = new int[2];
					pos_aux[0] = tmp[0];
					pos_aux[1] = tmp[1] + 1;
					updateBoard1.Add(pos_aux);
					myQ.Add(pos_aux);
					myDomination++;
				}
				if(tmp[1] - 1 >= 0 && auxBoard[tmp[0], tmp[1] - 1] == false && notInserted(tmp[0] , tmp[1] - 1, updateBoard1)) {
					// Console.WriteLine("Entrou caso 4");
					pos_aux = new int[2];
					pos_aux[0] = tmp[0];
					pos_aux[1] = tmp[1] - 1;
					updateBoard1.Add(pos_aux);
					myQ.Add(pos_aux);
					myDomination++;
				}
				myStart++;
			}
			aux = enemyQ.Count;
			for(int i = enemyStart; i < aux; i++) {
				tmp = new int[2];
				tmp = enemyQ[i];
				if(tmp[0] + 1 < boardSizeX && auxBoard[tmp[0] + 1, tmp[1]] == false && notInserted(tmp[0] + 1, tmp[1], updateBoard2)) {
					pos_aux = new int[2];
					pos_aux[0] = tmp[0] + 1;
					pos_aux[1] = tmp[1];
					updateBoard2.Add(pos_aux);
					enemyQ.Add(pos_aux);
					enemyDomination++;
				}
				if(tmp[0] - 1 >= 0 && auxBoard[tmp[0] - 1, tmp[1]] == false && notInserted(tmp[0] - 1, tmp[1], updateBoard2)) {
					pos_aux = new int[2];
					pos_aux[0] = tmp[0] - 1;
					pos_aux[1] = tmp[1];
					updateBoard2.Add(pos_aux);
					enemyQ.Add(pos_aux);
					enemyDomination++;
				}
				if(tmp[1] + 1 < boardSizeY && auxBoard[tmp[0], tmp[1] + 1] == false && notInserted(tmp[0] , tmp[1] + 1, updateBoard2)) {
					pos_aux = new int[2];
					pos_aux[0] = tmp[0];
					pos_aux[1] = tmp[1] + 1;
					updateBoard2.Add(pos_aux);
					enemyQ.Add(pos_aux);
					enemyDomination++;
				}
				if(tmp[1] - 1 >= 0 && auxBoard[tmp[0], tmp[1] - 1] == false && notInserted(tmp[0] , tmp[1] - 1, updateBoard2)) {
					pos_aux = new int[2];
					pos_aux[0] = tmp[0];
					pos_aux[1] = tmp[1] - 1;
					updateBoard2.Add(pos_aux);
					enemyQ.Add(pos_aux);
					enemyDomination++;
				}
				enemyStart++;
			}
			int aux1 = updateBoard1.Count;
			int aux2 = updateBoard2.Count;
			for(int i = 0; i < aux1; i++) {
				for(int j = 0; j < aux2; j++) {
					if(updateBoard1[i][0] == updateBoard2[j][0] && updateBoard1[i][1] == updateBoard2[j][1]) {
						enemyDomination--;
						myDomination--;
					}
				}
			}
			for(int i = 0; i < aux1; i++) {
				auxBoard[updateBoard1[i][0], updateBoard1[i][1]] = true;
			}
			updateBoard1.Clear();
			for(int i = 0; i < aux2; i++) {
				auxBoard[updateBoard2[i][0], updateBoard2[i][1]] = true;
			}
			updateBoard2.Clear();
			// Console.WriteLine(myDomination);
			// Console.WriteLine(enemyDomination);
			
			// for(int i = 0; i < boardSizeX; i++){
			// 	for(int j = 0; j < boardSizeY; j++){
			// 		if(auxBoard[i,j]) {
			// 			auxs += "1";
			// 		} else {
			// 			auxs += "0";
			// 		}
			// 	}
			// 	Console.WriteLine(auxs);
			// 	auxs = "";
			// }
			// aux = myQ.Count;
			// for(int i = myStart; i < aux; i++) {
			// 	Console.WriteLine("valor na myqueue: " + myQ[i][0] + " " + myQ[i][1]);
			// }
			// aux = enemyQ.Count;
			// for(int i = enemyStart; i < aux; i++) {
			// 	Console.WriteLine("valor na enemyqueue: " + enemyQ[i][0] + " " + enemyQ[i][1]);
			// }
		}

			// for(int i = 0; i < boardSizeX; i++){
			// 	for(int j = 0; j < boardSizeY; j++){
			// 		if(auxBoard[i,j]) {
			// 			auxs += "1";
			// 		} else {
			// 			auxs += "0";
			// 		}
			// 	}
			// 	Console.WriteLine(auxs);
			// 	auxs = "";
			// }
		return(myDomination - enemyDomination);
	}

	public static bool notInserted(int positionX, int positionY, List<int[]> updateBoard) {
		int aux = updateBoard.Count;
		for(int i = 0; i < aux; i++) {
			// Console.WriteLine("updateBoard: " + updateBoard[i][0] + " " + updateBoard[i][1]);
			// Console.WriteLine("position: " + positionX + " " + positionY);
			if(updateBoard[i][0] == positionX && updateBoard[i][1] == positionY) {
					// Console.WriteLine("___________________________________");
				return false;
			}
		}
		// Console.WriteLine("___________________________________");
		return true;
	}

	/*public bool won(int path, int path_size, int position_x, int position_y) {
		//deep search to find if enemy has less options to walk them you and them path_size
		//is it worth it?
	}*/

	public bool lost(int[] path, int path_size, int position_x, int position_y, bool[,] board) {
		bool[,] auxBoard = new bool[boardSizeX, boardSizeY];
		for(int i = 0; i < boardSizeX; i++){
			for(int j = 0; j < boardSizeY; j++){
				auxBoard[i,j] = board[i,j];
			}
		}
		for(int i = 0; i < path_size; i++) {
			//Debug.Log("path[" + i + "]= " + path[i]);
			switch(path[i]) {
				case 2:
					// Debug.Log("CASE 2: position X -> " + (position_x-1) + " || position Y -> " + position_y);
					// Debug.Break();
					if((position_x - 1) < 0 || position_y < 0 || position_y >= boardSizeY || auxBoard[position_x - 1,position_y] == true) {
						//Debug.Log("Lostcase1");
						return true;
					}else {
						auxBoard[position_x,position_y] = true;
						position_x--;
					}
					break;
				case 3:
					// Debug.Log("CASE 3: position X -> " + position_x + " || position Y -> " + (position_y+1));
					// Debug.Break();
					if((position_y + 1) >= boardSizeY || position_x < 0 || position_x >= boardSizeX || auxBoard[position_x,position_y + 1] == true) {
						// Debug.Log("Lostcase2");
						return true;
					} else {
						auxBoard[position_x,position_y] = true;
						position_y++;
					}
					break;
				case 0:
					// Debug.Log("CASE 0: position X -> " + (position_x+1) + " || position Y -> " + position_y);
					// Debug.Break();
					if((position_x + 1) >= boardSizeX || position_y < 0 || position_y >= boardSizeY || auxBoard[position_x + 1,position_y] == true) {
						// Debug.Log("Lostcase3");
						return true;
					} else {
						auxBoard[position_x,position_y] = true;
						position_x++;
					}
					break;
				case 1:
					// Debug.Log("CASE 0: position X -> " + position_x + " || position Y -> " + (position_y-1));
					// Debug.Break();
					if((position_y - 1) < 0 || position_x < 0 || position_x >= boardSizeX || auxBoard[position_x,position_y - 1] == true) {
						// Debug.Log("Lostcase4");
						return true;
					} else {
						auxBoard[position_x,position_y] = true;
						position_y--;
					}
					break;
				default:
					Console.WriteLine("Error: impossible path");
					break;
			}
		}
		return false;
	}
}