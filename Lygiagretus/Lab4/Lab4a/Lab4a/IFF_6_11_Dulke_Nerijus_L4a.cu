// IFF-6/11 Nerijus Dulke L4a
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <string>

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

	// host konstruktorius kvieciamas is CPU
	__host__ Automobilis() : galia(0), kuroSanaudos(0.0) {
		memset(pavadinimas, ' ', N * MAX_ILGIS - 1);
		pavadinimas[N * MAX_ILGIS] = '\0';
	}

	// device konstruktorius kvieciamas is GPU
	__device__ Automobilis(char pavadinimas[], int galia, double kuroSanaudos) {
		for (int i = 0; i < N * MAX_ILGIS; i++)
		{
			this->pavadinimas[i] = pavadinimas[i];
		}
		this->galia = galia;
		this->kuroSanaudos = kuroSanaudos;
	}

	// destruktorius kvieciamas is CPU ir GPU
	__host__ __device__ ~Automobilis() {};
};


// funkcija skirta sudeti masyvu elementu lauku reiksmes
__global__ void sudeti(Automobilis* automobiliai, Automobilis* rezultatai) {
	// paimamas gijos indeksas
	int index = threadIdx.x;
	int galia = 0;
	double kuroSanaudos = 0.0;
	char pavadinimai[N * MAX_ILGIS];

	for (int i = 0; i < N; i++)
	{
		// kadangi duomenys yra viename masyve o ne matricojse,
		// [i * K + index] yra atitinkamas elementas is masyvo
		galia += automobiliai[i * K + index].galia;
		kuroSanaudos += automobiliai[i * K + index].kuroSanaudos;

		for (int j = 0; j < MAX_ILGIS; j++)
		{
			pavadinimai[MAX_ILGIS * i + j] = automobiliai[i * K + index].pavadinimas[j];
		}
	}

	rezultatai[index] = Automobilis(pavadinimai, galia, kuroSanaudos);
}

cudaError_t vykdyti(Automobilis** duomenu_matrica, Automobilis* rezultatai) {
	cudaError_t status;

	// GPU kintamieji
	Automobilis* device_rezultatai = new Automobilis[K];
	Automobilis* device_duomenys = new Automobilis[K * N];

	// duomenys perkeliami is matricos i masyva
	Automobilis* duomenu_masyvas = new Automobilis[K * N];
	for (int i = 0; i < N; i++)
	{
		for (int j = 0; j < K; j++)
		{
			duomenu_masyvas[i * K + j] = duomenu_matrica[i][j];
		}
	}

	// Pasirenkamas GPU irenginys
	status = cudaSetDevice(0);
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida pasirenkant GPU");
		goto Error;
	}

	// Paskiriama atmintis GPU
	status = cudaMalloc((void**)&device_duomenys, N * K * sizeof(Automobilis));
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida paskiriant atminti");
		goto Error;
	}
	status = cudaMalloc((void**)&device_rezultatai, K * sizeof(Automobilis));
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida paskiriant atminti");
		goto Error;
	}

	// Nukopijuojami duomenys i GPU
	status = cudaMemcpy(device_duomenys, duomenu_masyvas, N * K * sizeof(Automobilis), cudaMemcpyHostToDevice);
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida kopijuojant i GPU");
		goto Error;
	}
	status = cudaMemcpy(device_rezultatai, rezultatai, K * sizeof(Automobilis), cudaMemcpyHostToDevice);
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida kopijuojant i GPU");
		goto Error;
	}

	// vykdoma 1 giju bloke, naudojant K giju
	sudeti<<<1, K>>>(device_duomenys, device_rezultatai);

	// patikrinama ar vykdant sudeti atsirado klaidu
	status = cudaGetLastError();
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida vykdant sudeti");
		goto Error;
	}
	
	// laukiama vykdymo pabaigos
	status = cudaDeviceSynchronize();
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida sinchronizuojant");
		goto Error;
	}

	// kuopijuojami rezultatai i CPU
	status = cudaMemcpy(rezultatai, device_rezultatai, K * sizeof(Automobilis), cudaMemcpyDeviceToHost);
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida kopijuojant i CPU");
		goto Error;
	}

// ivykus klaidai atlaisvinama atmintis
Error:
	delete[] duomenu_masyvas;
	cudaFree(device_duomenys);
	cudaFree(device_rezultatai);

	return status;
}

// funkcija skirta skaityti duomenims is failo
void skaityti(Automobilis** automobiliai) {
	ifstream F("IFF_6_11_Dulke_Nerijus_L4.txt");
	string pavadinimas;

	for (int i = 0; i < N; i++)
	{
		Automobilis* automobiliai_temp = new Automobilis[K];
		
		F.ignore();
		for (int j = 0; j < K; j++)
		{
			F >> pavadinimas;
			for (unsigned int k = 0; k < pavadinimas.length(); k++)
			{
				automobiliai_temp[j].pavadinimas[k] = pavadinimas[k];
			}
			F >> automobiliai_temp[j].galia >> automobiliai_temp[j].kuroSanaudos;
			F.ignore();
		}

		automobiliai[i] = automobiliai_temp;
	}

	F.close();
}

// funkcija skirta spausdinti pradinius duomenis i faila
void spausdintiDuomenis(Automobilis** automobiliai) {
	ofstream F("IFF_6_11_Dulke_Nerijus_L4a_rez.txt");
	for (int i = 0; i < N; i++)
	{
		F << "    ----- Automobiliu masyvas Nr. " << (i + 1) << " ----------" << endl;
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		F << "   |" << setw(MAX_ILGIS) << left << "Pavadinimas" << setw(13) << left << "|Galia" << setw(9) << left << "|Kuro sanaudos|" << endl;
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		for (int j = 0; j < K; j++) {
			F << setw(3) << left << (j + 1) << "|";
			for (int k = 0; k < MAX_ILGIS; k++) F << automobiliai[i][j].pavadinimas[k];
			F << "|" << setw(12) << left << automobiliai[i][j].galia << "|";
			F << setw(13) << left << fixed << setprecision(2) << automobiliai[i][j].kuroSanaudos << "|" << endl;
		}
		F << "   |" << string(MAX_ILGIS, '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
		F << endl;
	}
}

// funkcija skirta spausdinti rezultatus i faila
void spausdintiRezultatus(Automobilis* automobiliai) {
	ofstream F("IFF_6_11_Dulke_Nerijus_L4a_rez.txt", ios::app);
	int lineNr = 1;
	F << "   ************" << endl;
	F << "    Rezultatai" << endl;
	F << "   ************" << endl;
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	F << "   |" << setw(N * MAX_ILGIS) << left << "Sujungti pavadinimai" << setw(13) << left << "|Galia" << setw(9) << left << "|Kuro sanaudos|" << endl;
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	for (int i = 0; i < K; i++) {
		F << setw(3) << left << lineNr++ << "|" << setw(N * MAX_ILGIS) << left << automobiliai[i].pavadinimas;
		F << "|" << setw(12) << left << automobiliai[i].galia << "|";
		F << setw(13) << left << fixed << setprecision(2) << automobiliai[i].kuroSanaudos << "|" << endl;
	}
	F << "   |" << string((N * MAX_ILGIS), '-') << "|" << string(12, '-') << "|" << string(13, '-') << "|" << endl;
	F.close();
}

int main() {
	// dvimatis duomenu masyvas
	Automobilis** automobiliai = new Automobilis*[N];
	Automobilis* rezultatai = new Automobilis[K];

	skaityti(automobiliai);

	// vykdom pagrindine funkcija ir tikrinama ar neivyko klaidu
	cudaError_t status = vykdyti(automobiliai, rezultatai);
	if (status != cudaSuccess) {
		fprintf(stderr, "Ivyko klaida");
		return 1;
	}

	//atspausdinami duomenys ir rezultatai
	spausdintiDuomenis(automobiliai);
	spausdintiRezultatus(rezultatai);

	// atlaisvinama atmintis
	delete[] automobiliai;
	delete[] rezultatai;

	return 0;
}
