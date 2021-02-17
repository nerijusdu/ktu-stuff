// IFF-6/11 Nerijus Dulke L4b
#include <stdio.h>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>
#include <thrust/host_vector.h>
#include <thrust/device_vector.h>

using namespace std;

// masyvu skaicius
const int N = 4;
// automobiliu skaicius masyve
const int K = 10;
// maksimalus pavadinimo simboliu skaicius
const int MAX_ILGIS = 15;

struct Automobilis {
public: 
	char pavadinimas[N * MAX_ILGIS + 1];
	int galia;
	double kuroSanaudos;

	// konstruktorius kvieciamas is CPU arba GPU
	__host__ __device__ Automobilis() : galia(0), kuroSanaudos(0.0) {
		memset(pavadinimas, ' ', N * MAX_ILGIS - 1);
		pavadinimas[N * MAX_ILGIS] = '\0';
	};

	// konstruktorius kvieciamas is CPU arba GPU
	__device__ __host__ Automobilis(char pavadinimas[], int galia, double kuroSanaudos) {
		for (int i = 0; i < N * MAX_ILGIS; i++)
		{
			this->pavadinimas[i] = pavadinimas[i];
		}
		this->galia = galia;
		this->kuroSanaudos = kuroSanaudos;
	};

	// destruktorius kvieciamas is CPU arba GPU
	__host__ __device__ ~Automobilis() {};
};


// funkcija skirta sudeti masyvu elementu lauku reiksmes
Automobilis sudeti(int id, thrust::device_vector<Automobilis>::iterator dev_iter_start) {
	// pradzios iteratoriui priskiriama atitinkamas vektoriaus elementas
	thrust::device_vector<Automobilis>::iterator iter = dev_iter_start + id;
	int galia = 0;
	double kuroSanaudos = 0.0;
	char pavadinimai[N * MAX_ILGIS];

	for (int i = 0; i < N; i++)
	{
		// paimamas automobilio objektas
		Automobilis temp = (static_cast<Automobilis>(*iter));

		galia += temp.galia;
		kuroSanaudos += temp.kuroSanaudos;
		for (int j = 0; j < MAX_ILGIS; j++)
		{
			pavadinimai[MAX_ILGIS * i + j] = temp.pavadinimas[j];
		}

		// iteratorius pereina i kita eilute (kuri yra uz K poziciju)
		iter += K;
	}

	return Automobilis(pavadinimai, galia, kuroSanaudos);
}

// funkcija skirta skaityti duomenims is failo
void skaityti(thrust::host_vector<Automobilis> &automobiliai) {
	ifstream F("IFF_6_11_Dulke_Nerijus_L4.txt");
	string pavadinimas;

	for (int i = 0; i < N; i++)
	{
		F.ignore();
		for (int j = 0; j < K; j++)
		{
			Automobilis automobilis_temp = Automobilis();

			F >> pavadinimas;
			for (unsigned int k = 0; k < pavadinimas.length(); k++)
			{
				automobilis_temp.pavadinimas[k] = pavadinimas[k];
			}
			F >> automobilis_temp.galia >> automobilis_temp.kuroSanaudos;
			automobiliai.push_back(automobilis_temp);
			
			F.ignore();
		}
	}

	F.close();
}

// funkcija skirta spausdinti pradinius duomenis i faila
void spausdintiDuomenis(thrust::host_vector<Automobilis> &automobiliai) {
	ofstream F("IFF_6_11_Dulke_Nerijus_L4b_rez.txt");
	for (int i = 0; i < N; i++)
	{
		F << "   ------ Automobiliu masyvas Nr. " << (i + 1) << " ----------" << endl;
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		F << "   |" << setw(MAX_ILGIS) << left << "Pavadinimas" << setw(13) << left << "|Galia" << setw(9) << left << "|Kuro sanaudos|" << endl;
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		for (int j = 0; j < K; j++) {
			F << setw(3) << left << (j + 1) << "|";
			for (int k = 0; k < MAX_ILGIS; k++) F << automobiliai[i * K + j].pavadinimas[k];
			F << "|" << setw(12) << left << automobiliai[i * K + j].galia << "|";
			F << setw(13) << left << fixed << setprecision(2) << automobiliai[i * K + j].kuroSanaudos << "|" << endl;
		}
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		F << endl;
	}
}

// funkcija skirta spausdinti rezultatus i faila
void spausdintiRezultatus(thrust::host_vector<Automobilis> &automobiliai) {
	ofstream F("IFF_6_11_Dulke_Nerijus_L4b_rez.txt", ios::app);
	F << "   ************" << endl;
	F << "    Rezultatai" << endl;
	F << "   ************" << endl;
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	F << "   |" << setw(N * MAX_ILGIS) << left << "Sujungti pavadinimai" << setw(13) << left << "|Galia" << setw(9) << left << "|Kuro sanaudos|" << endl;
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	for (int i = 0; i < K; i++) {
		F << setw(3) << left << (i + 1) << "|";// << setw(N * MAX_ILGIS) << left << automobiliai[i].pavadinimas;
		for (int j = 0; j < N * MAX_ILGIS; j++) {
			F << automobiliai[i].pavadinimas[j];
		}
		F << "|" << setw(12) << left << automobiliai[i].galia << "|";
		F << setw(13) << left << fixed << setprecision(2) << automobiliai[i].kuroSanaudos << "|" << endl;
	}
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	F.close();
}

int main() {
	// sukuriami CPU (host) ir GPU (device) duomenu ir rezultatu vektoriai
	thrust::host_vector<Automobilis> automobiliai;
	thrust::host_vector<Automobilis> rezultatai;
	thrust::device_vector<Automobilis> dev_automobiliai;
	thrust::device_vector<Automobilis> dev_rezultatai(K);

	// nuskaitomi pradiniai duomenys
	skaityti(automobiliai);

	// duomenys nukopijuojami is CPU i GPU atminti
	dev_automobiliai = automobiliai;

	// sukuriamas GPU atmintyje esanciu duomenu pradzios iteratorius
	thrust::device_vector<Automobilis>::iterator dev_iter_start = dev_automobiliai.begin();

	// sujungiami kiekvieno proceso duomenu laukai 
	for (int i = 0; i < K; i++)
	{
		dev_rezultatai[i] = sudeti(i, dev_iter_start);
	}

	// rezultatai kopijuojami atgal is GPU i CPU atminti
	rezultatai = dev_rezultatai;

	// atspausdinami pradiniai duomenys ir rezultatai
	spausdintiDuomenis(automobiliai);
	spausdintiRezultatus(rezultatai);

	return 0;
}
