// L2a, Nerijus Dulke, IFF-6/11

#include "pch.h"
#include <iostream>
#include <fstream>
#include <sstream>
#include <string>
#include <mutex> 
#include <atomic>
#include <iomanip>
#include <thread>

using namespace std;

// duomenu failo pasirinkimas ------------------------------
const int PasirinktasDuomFailas = 2; // 0, 1 arba 2
// ---------------------------------------------------------

const string SkaitytojuFailai[] = { "IFF_6_11_Dulke_Nerijus_L2_dat_1.txt", "IFF_6_11_Dulke_Nerijus_L2_dat_2.txt", "IFF_6_11_Dulke_Nerijus_L2_dat_3.txt" };
const string RezultatuFailas = "IFF_6_11_Dulke_Nerijus_L2a_rez.txt";
const int GijuKiekis = 4;
const int SarasoDydis = 22;

struct Automobilis {
	string pavadinimas;
	int galia;
	double kuroSanaudos;
	int kiekis = 1;
};

struct Salonas {
	string pavadinimas;
	Automobilis *automobiliai;
	int kiekis;
};

struct SalonuSarasas {
	Salonas *salonai = new Salonas[10];
	int kiekis = 0;
};

struct Sarasas {
private:
	Automobilis *automobiliai;
	int maxKiekis;
	atomic<int> baigeIrasyma = 0;
	atomic<int> baigeTrynima = 0;
	mutex mtx;
public:
	atomic<int> kiekis;
	atomic<bool> turiElementu = false;
	condition_variable imti;

	Sarasas(int dydis) : maxKiekis(dydis) {
		kiekis = 0;
		automobiliai = new Automobilis[maxKiekis];
	}

	// ar visi rasytojai baige rasyma
	bool VisiBaigeRasyma() {
		return baigeIrasyma == GijuKiekis;
	}
	void BaigeTrinti() {
		baigeTrynima++;
	}
	void BaigeRasyti() {
		baigeIrasyma++;
		if (VisiBaigeRasyma()) {
			imti.notify_all();
		}
	}

	//pridedamas elementas i sarasa
	bool Prideti(Automobilis naujas) {
		mtx.lock();

		//Patikrinama ar toks automobilis jau yra sarase
		for (int i = 0; i < kiekis; i++) {
			if (automobiliai[i].pavadinimas == naujas.pavadinimas) {
				automobiliai[i].kiekis++;
				mtx.unlock();
				return true;
			}
		}
		//Tesiama jei automobilio sarase nera

		//Tikrinama ar sarasas pilnas
		if (kiekis == maxKiekis) {
			mtx.unlock();
			return false;
		}
		
		for (int i = 0; i < kiekis; i++)
		{
			if (automobiliai[i].pavadinimas < naujas.pavadinimas) {
				// Perstumiamas sarasas
				for (int j = kiekis; j > i; j--)
				{
					automobiliai[j] = automobiliai[j - 1];
				}
				automobiliai[i] = { naujas.pavadinimas };
				kiekis++;

				mtx.unlock();
				return true;
			}
		}

		//Pridedama i saraso gala
		automobiliai[kiekis] = { naujas.pavadinimas };
		kiekis++;
		turiElementu = true;
		imti.notify_all();

		mtx.unlock();
		return true;
	}

	//salinamas elementas is saraso
	bool Salinti(Automobilis salinamas) {
		mtx.lock();

		// Ieskomas reikiamas elementas sarase
		for (int i = 0; i < kiekis; i++)
		{
			if (automobiliai[i].pavadinimas == salinamas.pavadinimas)
			{
				automobiliai[i].kiekis--;
				if (automobiliai[i].kiekis == 0)
				{
					// Perstumiami elementai
					for (int a = i + 1; a < kiekis; a++)
					{
						automobiliai[a - 1] = automobiliai[a];
					}

					kiekis--;
					if (kiekis == 0)
					{
						turiElementu = false;
					}
				}

				mtx.unlock();
				return true;
			}
		}

		mtx.unlock();
		return false;
	}

	Automobilis GautiElementa(int index) {
		return automobiliai[index];
	}
};

Sarasas galutinisSarasas(SarasoDydis);

// nuskaitomi duomenys is failo, grazina sarasus su duomenimis
void SkaitytiDuomenis(SalonuSarasas *pridedama, SalonuSarasas *trinama, string failoPavadinimas) {
	int likesSalonuKiekis = GijuKiekis;
	ifstream duomenuFailas(failoPavadinimas);
	string eilute = "";
	int eilNr = 0;
	int likesKiekis = 0;
	Salonas salonas;

	// skaitomos visos eilutes
	while (getline(duomenuFailas, eilute)) {
		stringstream stream(eilute);
		int stulpNr = 0;
		string value = "";

		// eilutes suskaldomos i stulpelius pagal ','
		while (getline(stream, value, ',')) {
			// jei likesKiekis > 0, bus nuskaitoma automobilio informacija
			if (likesKiekis > 0) {
				switch (stulpNr)
				{
					case 0:
						salonas.automobiliai[eilNr].pavadinimas = value;
						break;
					case 1:
						string::size_type dydis;
						// skaitytojai turi vienu stulpeliu maziau
						if (likesSalonuKiekis >= 0) {
							salonas.automobiliai[eilNr].galia = stoi(value, &dydis);
						}
						else {
							salonas.automobiliai[eilNr].kiekis = stoi(value, &dydis);
							likesKiekis--;
							eilNr++;
						}
						break;
					case 2:
						salonas.automobiliai[eilNr].galia = stod(value, &dydis);
						likesKiekis--;
						eilNr++;
						break;
				}
			}
			// jei likesKiekis <= 0, bus nuskaitoma salono informacija
			else {
				switch (stulpNr)
				{
					case 0:
						salonas.pavadinimas = value;
						break;
					case 1:
						string::size_type dydis;
						int kiekis = stoi(value, &dydis);

						salonas.automobiliai = new Automobilis[kiekis];
						salonas.kiekis = kiekis;
						if (likesSalonuKiekis > 0) {
							pridedama->salonai[pridedama->kiekis] = salonas;
							pridedama->kiekis++;
						}
						else {
							trinama->salonai[trinama->kiekis] = salonas;
							trinama->kiekis++;
						}

						likesKiekis = kiekis;
						eilNr = 0;
						likesSalonuKiekis--;
						break;
				}
			}
			stulpNr++;
		}
	}
	duomenuFailas.close();
}

// isvedami vieno salono duomenys i faila
void IsvestiSalona(const Salonas& salonas, const string tipas) {
	ofstream failas;
	failas.open(RezultatuFailas, ios::out | ios::app);

	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;
	failas << setw(2) << left << "|" << left << setw(56) << salonas.pavadinimas + tipas << "|" << endl;
	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;

	failas
		<< setw(2) << left << "|" << left << setw(5) << "Id"
		<< setw(2) << right << "|" << right << setw(35) << "Automobilis"
		<< setw(3) << right << "| " << left << setw(10) << "Kiekis" << setw(2) << right << "|" << endl;

	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;

	for (int i = 0; i < salonas.kiekis; i++)
	{
		failas
			<< setw(2) << left << "|" << left << setw(5) << i + 1
			<< setw(2) << right << "|" << right << setw(35) << salonas.automobiliai[i].pavadinimas
			<< setw(3) << right << "| " << left << setw(10) << salonas.automobiliai[i].kiekis << setw(2) << right << "|" << endl;
	}

	failas.close();
}

// isvedami pradiniai duomenis i rezultatu faila
void IsvestiPradiniusDuomenis(const SalonuSarasas& salonuSarasas, const string tipas) {
	for (int i = 0; i < salonuSarasas.kiekis; i++)
	{
		IsvestiSalona(salonuSarasas.salonai[i], tipas);
	}
}

bool TikrintiSarasa()
{
	return galutinisSarasas.turiElementu || galutinisSarasas.VisiBaigeRasyma();
}

// pasalinami salone esantys automobiliai is galutinio saraso
void SalintiIsGalutinioSaraso(Salonas salonas) {
	mutex lock;
	unique_lock<mutex> apsauga(lock);

	bool baigtiSarasai = false;
	while (salonas.kiekis > 0 && !baigtiSarasai) {
		baigtiSarasai = galutinisSarasas.VisiBaigeRasyma();

		for (int i = 0; i < salonas.kiekis; i++)
		{
			galutinisSarasas.imti.wait(apsauga, TikrintiSarasa);
			
			// Jei elementas sekmingai pasalintas
			if (galutinisSarasas.Salinti(salonas.automobiliai[i])) {
				salonas.automobiliai[i].kiekis--;
				if (salonas.automobiliai[i].kiekis == 0) {
					//Perstumiami elementai
					for (int j = i + 1; j < salonas.kiekis; j++)
					{
						salonas.automobiliai[j - 1] = salonas.automobiliai[j];
					}
					salonas.kiekis--;
				}
				i--;
			}
		}
	}

	galutinisSarasas.BaigeTrinti();
}

// salone esantys automobiliai pridedami i galutini sarasa
void PridetiIGalutiniSarasa(Salonas salonas) {
	mutex lock;
	unique_lock<mutex> apsauga(lock);

	while (salonas.kiekis > 0) {
		for (int i = 0; i < salonas.kiekis; i++)
		{
			if (galutinisSarasas.Prideti(salonas.automobiliai[i])) {
				// perstumiami elementai
				for (int a = i + 1; a < salonas.kiekis; a++)
				{
					salonas.automobiliai[a - 1] = salonas.automobiliai[a];
				}
				salonas.kiekis--;
				i--;
			}
		}
	}

	galutinisSarasas.BaigeRasyti();
}

// galutinis sarasas isvedamas i rezultatu faila
void IsvestiGalutiniSarasa(Sarasas &sarasas) {
	ofstream failas;
	failas.open(RezultatuFailas, ios::out | ios::app);

	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;
	failas << setw(2) << left << "|" << left << setw(56) << "Galutinis sarasas" << "|" << endl;
	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;

	failas
		<< setw(2) << left << "|" << left << setw(5) << "Id"
		<< setw(2) << right << "|" << right << setw(35) << "Automobilis"
		<< setw(3) << right << "| " << left << setw(10) << "Kiekis" << setw(2) << right << "|" << endl;

	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;

	for (int i = 0; i < sarasas.kiekis; i++)
	{
		failas
			<< setw(2) << left << "|" << left << setw(5) << i + 1
			<< setw(2) << right << "|" << right << setw(35) << sarasas.GautiElementa(i).pavadinimas
			<< setw(3) << right << "| " << left << setw(10) << sarasas.GautiElementa(i).kiekis << setw(2) << right << "|" << endl;
	}
	failas << setfill('-') << setw(59) << '-' << setfill(' ') << endl;
	failas.close();
}

int main() {
	// isvalomas rezultatu failas
	ofstream out;
	out.open(RezultatuFailas, ios::out);
	out.close();

	//nuskaitomi ir atspausdinami pradiniai duomenys
	SalonuSarasas pridedamiAuto;
	SalonuSarasas salinamiAuto;
	SkaitytiDuomenis(&pridedamiAuto, &salinamiAuto, SkaitytojuFailai[PasirinktasDuomFailas]);
	IsvestiPradiniusDuomenis(pridedamiAuto, " - pridedama");
	IsvestiPradiniusDuomenis(salinamiAuto, " - salinama");

	// sukuriamos ir paleidziamos gijos
	std::thread *salinimoGijos = new thread[salinamiAuto.kiekis];
	std::thread *pridejimoGijos = new thread[pridedamiAuto.kiekis];
	for (int i = 0; i < salinamiAuto.kiekis; i++)
	{
		salinimoGijos[i] = std::thread(SalintiIsGalutinioSaraso, salinamiAuto.salonai[i]);
	}
	for (int i = 0; i < pridedamiAuto.kiekis; i++)
	{
		pridejimoGijos[i] = std::thread(PridetiIGalutiniSarasa, pridedamiAuto.salonai[i]);
	}

	// palaukiama kol gijos uzbaigs darba
	for (int i = 0; i < pridedamiAuto.kiekis; i++)
	{
		pridejimoGijos[i].join();
	}
	for (int i = 0; i < salinamiAuto.kiekis; i++)
	{
		salinimoGijos[i].join();
	}

	// isvedami rezultatai
	IsvestiGalutiniSarasa(galutinisSarasas);
}