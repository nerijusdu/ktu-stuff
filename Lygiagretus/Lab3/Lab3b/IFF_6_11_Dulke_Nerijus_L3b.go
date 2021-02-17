// Nerijus Dulke IFF-6/11 Lab3a
package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

type Automobilis struct {
	pavadinimas  string
	galia        int
	kuroSanaudos float64
}

type AutoStruktura struct {
	pavadinimas string
	kiekis      int
}

type Salonas struct {
	pavadinimas  string
	automobiliai []Automobilis
	kiekis       int
}

type Pirkejas struct {
	vardas       string
	automobiliai []AutoStruktura
	kiekis       int
}

type Zinute struct {
	pavadinimas string
	tipas       string
	paskutinis  bool
	indeksas    int
}

// salonu ir pirkeju kiekis tas pats
const N int = 4

// zinuciu tipai
const tSalinti string = "S"
const tPrideti string = "P"

// duomenu failo pavadinimas (pasirinkti tik viena)
const DuomenuFailas string = "IFF_6_11_Dulke_Nerijus_L3_dat_1.txt"

// const DuomenuFailas string = "IFF_6_11_Dulke_Nerijus_L3_dat_2.txt"

// const DuomenuFailas string = "IFF_6_11_Dulke_Nerijus_L3_dat_3.txt"

// rezultatu failo pavadinimas
const RezultatuFailas string = "IFF_6_11_Dulke_Nerijus_L3b_rez.txt"

// pagrindine funkcija kurioje sukuriami ir startuojami procesai
func main() {
	salonuKanalai := make([]chan Salonas, N)
	for i := 0; i < N; i++ {
		salonuKanalai[i] = make(chan Salonas)
	}

	pirkejuKanalai := make([]chan Pirkejas, N)
	for i := 0; i < N; i++ {
		pirkejuKanalai[i] = make(chan Pirkejas)
	}

	var trynimoPabaigosKanalai = make([]chan bool, N)
	for i := 0; i < N; i++ {
		trynimoPabaigosKanalai[i] = make(chan bool)
	}

	var rasymoValdymoKanalai = make([]chan Zinute, N)
	for i := range rasymoValdymoKanalai {
		rasymoValdymoKanalai[i] = make(chan Zinute)
	}

	var skaitymoValdymoKanalai = make([]chan Zinute, N)
	for i := range skaitymoValdymoKanalai {
		skaitymoValdymoKanalai[i] = make(chan Zinute)
	}

	var pabaigosKanalas = make(chan bool)
	var duomenuKanalas = make(chan []AutoStruktura)

	// startuojami rasytoju kanalai
	for i, salonoKanalas := range salonuKanalai {
		go salonasParduoda(salonoKanalas, rasymoValdymoKanalai[i], i)
	}

	// startuoja vartotoju procesai
	for i, pirkejoKanalas := range pirkejuKanalai {
		go pirkejasPerka(pirkejoKanalas, skaitymoValdymoKanalai[i], trynimoPabaigosKanalai[i], i)
	}

	// startuoja duomenu procesas
	go duomenuSkirstymas(salonuKanalai, pirkejuKanalai, pabaigosKanalas, duomenuKanalas)

	// startuoja valdytojas
	go valdytojas(rasymoValdymoKanalai, skaitymoValdymoKanalai, duomenuKanalas, trynimoPabaigosKanalai)
	<-pabaigosKanalas
}

// salono automobiliai perduodami valdytojui kad pridetu i pagrindini sarasa
func salonasParduoda(pirkejoKanalas <-chan Salonas, valdymoKanalas chan<- Zinute, indeksas int) {
	// gauna duomenu rinkini
	var salonas = <-pirkejoKanalas
	for i, automobilis := range salonas.automobiliai {
		paskutinis := i == salonas.kiekis-1

		valdymoKanalas <- Zinute{
			pavadinimas: automobilis.pavadinimas,
			tipas:       tPrideti,
			paskutinis:  paskutinis,
			indeksas:    indeksas}
	}
}

// pirkejo automobiliai perduodami valdytojui kad pasalintu is pagrindinio saraso
func pirkejasPerka(pirkejoKanalas <-chan Pirkejas, valdymoKanalas chan<- Zinute, trynimoPabaigosKanalai <-chan bool, indeksas int) {
	// gauna duomenu rinkini
	var pirkejas = <-pirkejoKanalas

	// Jei pirkejas neturi prekiu indeksas grainamas -1
	if pirkejas.kiekis == 0 {
		valdymoKanalas <- Zinute{
			pavadinimas: "",
			tipas:       tSalinti,
			paskutinis:  true,
			indeksas:    -1}
		return
	}

	// ar visi rasytojai baige savo darba
	arBaigesiDuomenuPridejimas := false
	for !arBaigesiDuomenuPridejimas {
		for i, pirkinys := range pirkejas.automobiliai {
			isLastindeksas := i == pirkejas.kiekis-1
			for j := 0; j < pirkinys.kiekis; j++ {
				paskutinis := isLastindeksas && pirkinys.kiekis-1 == j
				// kiekviena elementa siuncia i valdytojo kanala salinimui
				valdymoKanalas <- Zinute{
					pavadinimas: pirkinys.pavadinimas,
					tipas:       tSalinti,
					paskutinis:  paskutinis,
					indeksas:    indeksas}
			}
		}
		// is valdytojo kanalo, pasiuntus paskutini elementa salinimui, suzinoma, ar dar sukti cikla
		arBaigesiDuomenuPridejimas = <-trynimoPabaigosKanalai
	}

	// jeigu pasikeitimai nebegalimi, paskutini karta bandoma viska istrinti
	for i, pirkinys := range pirkejas.automobiliai {
		paskutinis := i == pirkejas.kiekis-1
		valdymoKanalas <- Zinute{
			pavadinimas: pirkinys.pavadinimas,
			tipas:       tSalinti,
			paskutinis:  paskutinis,
			indeksas:    indeksas}
	}
}

// funkcija paskirstanti duomenis salonams ir pirkejams, bei laukianti rezultatu
func duomenuSkirstymas(salonoKanalas []chan Salonas,
	pirkejoKanalas []chan Pirkejas,
	pabaigosKanalas chan<- bool,
	duomenuKanalas <-chan []AutoStruktura) {
	salonai, pirkejai := failoNuskaitymas(DuomenuFailas)

	// siuncia pradini rinkini gamintojo procesui
	for i, salonas := range salonai {
		salonoKanalas[i] <- salonas
	}
	// siuncia pradini rinkini vartotojo procesui
	for i, pirkejas := range pirkejai {
		pirkejoKanalas[i] <- pirkejas
	}

	// laukia is valdytojo proceso galinio rezultato objekto
	var AutoStruktura = <-duomenuKanalas

	rezultatuSpausdinimasIFaila(salonai, pirkejai, AutoStruktura)

	pabaigosKanalas <- true
}

// valdytojas
func valdytojas(rasymoValdymoKanalai []chan Zinute,
	skaitymoValdymoKanalai []chan Zinute,
	duomenuSkirstymas chan<- []AutoStruktura,
	trynimoPabaigosKanalai []chan bool) {
	rezultatai := make([]AutoStruktura, 100)

	arPirkejasApsipirko := make([]bool, N)
	for i := range arPirkejasApsipirko {
		arPirkejasApsipirko[i] = false
	}

	baigusiuSalonuKiekis := 0

	baigusiuPirkejuKiekis := 0

	rezultatuKiekis := 0

	for baigusiuPirkejuKiekis != N || baigusiuSalonuKiekis != N {
		select {
		case zinute := <-rasymoValdymoKanalai[0]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis = prideti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis)
		case zinute := <-rasymoValdymoKanalai[1]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis = prideti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis)
		case zinute := <-rasymoValdymoKanalai[2]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis = prideti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis)
		case zinute := <-rasymoValdymoKanalai[3]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis = prideti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis)
		case zinute := <-skaitymoValdymoKanalai[0]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis = salinti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, trynimoPabaigosKanalai, baigusiuPirkejuKiekis)
		case zinute := <-skaitymoValdymoKanalai[1]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis = salinti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, trynimoPabaigosKanalai, baigusiuPirkejuKiekis)
		case zinute := <-skaitymoValdymoKanalai[2]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis = salinti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, trynimoPabaigosKanalai, baigusiuPirkejuKiekis)
		case zinute := <-skaitymoValdymoKanalai[3]:
			rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis = salinti(rezultatai, zinute, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, trynimoPabaigosKanalai, baigusiuPirkejuKiekis)
		}
	}
	duomenuSkirstymas <- rezultatai
}

// automobilio pridejimas i pagrindini sarasa
func prideti(rezultatai []AutoStruktura, zinute Zinute, rezultatuKiekis int, baigusiuSalonuKiekis int) ([]AutoStruktura, int, int) {
	// jei struktura tuscia, prideda i pradzia
	if rezultatuKiekis == 0 {
		rezultatai[0] = AutoStruktura{pavadinimas: zinute.pavadinimas, kiekis: 1}
		rezultatuKiekis++

		if zinute.paskutinis {
			baigusiuSalonuKiekis++
		}
		return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis
	}
	indeksas := 0
	gautasIdejimoIndeksas := false
	// iteruojama masyvu tol, kol randamas iterpimo indeksas arba pasiekiama laisva vieta irasu gale
	for !(rezultatai[indeksas].pavadinimas == "" && rezultatai[indeksas].kiekis == 0) && !gautasIdejimoIndeksas {
		if rezultatai[indeksas].pavadinimas >= zinute.pavadinimas {
			gautasIdejimoIndeksas = true
			break
		}

		indeksas++
	}

	// Jeigu randamas iterpimo vietos indeksas, iterpiama toje vietoje
	if gautasIdejimoIndeksas {
		// jeigu masyvo elementas duotame indekse sutampa, tai padidiname pasikartojimu skaiciu
		if rezultatai[indeksas].pavadinimas == zinute.pavadinimas {
			rezultatai[indeksas].kiekis = rezultatai[indeksas].kiekis + 1
		} else { // perstumiame elementus i desine ir iterpiame nauja elementa
			for i := rezultatuKiekis; i > indeksas; i-- {
				rezultatai[i] = rezultatai[i-1]
			}
			rezultatai[indeksas] = AutoStruktura{pavadinimas: zinute.pavadinimas, kiekis: 1}
			rezultatuKiekis++
		}
	} else { // jeigu nerandamas, irasas iterpiamas gale
		rezultatai[indeksas] = AutoStruktura{pavadinimas: zinute.pavadinimas, kiekis: 1}
		rezultatuKiekis++
	}

	if zinute.paskutinis {
		baigusiuSalonuKiekis++
	}

	return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis
}

// automobilio salinimas is pagrindinio saraso
func salinti(rezultatai []AutoStruktura,
	zinute Zinute,
	rezultatuKiekis int,
	baigusiuSalonuKiekis int,
	arPirkejasApsipirko []bool,
	trynimoPabaigosKanalai []chan bool,
	baigusiuPirkejuKiekis int) ([]AutoStruktura, int, int, []bool, int) {
	rezultatai, rezultatuKiekis = salintiElementa(rezultatai, zinute.pavadinimas, rezultatuKiekis)
	// jeigu paskutinis trinamas elementas cikle
	if zinute.indeksas == -1 {
		return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis + 1
	}

	if zinute.paskutinis {
		// ir jeigu visi gamintojai baige savo darba
		if baigusiuSalonuKiekis == N {
			// jeigu vartotojas pirma karta istrine paskutini elementa gamintojams baigus darba
			if !arPirkejasApsipirko[zinute.indeksas] {
				arPirkejasApsipirko[zinute.indeksas] = true
				trynimoPabaigosKanalai[zinute.indeksas] <- true
				return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis
			}

			baigusiuPirkejuKiekis++
		} else {
			trynimoPabaigosKanalai[zinute.indeksas] <- false
			return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis
		}
	}

	return rezultatai, rezultatuKiekis, baigusiuSalonuKiekis, arPirkejasApsipirko, baigusiuPirkejuKiekis
}

// pagalbine funkcija, naudojama funkcijoje 'salinti'
func salintiElementa(rezultatai []AutoStruktura, pavadinimas string, rezultatuKiekis int) ([]AutoStruktura, int) {
	// jeigu masyvas tuscias, niekas neistrinama
	if rezultatuKiekis == 0 {
		return rezultatai, 0
	}

	indeksas := 0
	rastasTrynimoIndeksas := false
	//i teruojama kol randamas elementas masyve arba pereinama per visus irasus
	for !(rezultatai[indeksas].pavadinimas == "" && rezultatai[indeksas].kiekis == 0) && !rastasTrynimoIndeksas {
		if rezultatai[indeksas].pavadinimas == pavadinimas {
			rastasTrynimoIndeksas = true
			break
		}

		indeksas++
	}
	//Jeigu elementas randamas ir jo pasikartojimu skaicius > 1, tai sumaziname skaiciu vienetu
	// jeigu elemento pasikartojimu skaicius 1, tai triname elementa is masyvo perstumadami visus irasus
	if rastasTrynimoIndeksas {
		if rezultatai[indeksas].kiekis > 1 {
			rezultatai[indeksas].kiekis = rezultatai[indeksas].kiekis - 1
			return rezultatai, rezultatuKiekis
		}

		for i := indeksas; i < rezultatuKiekis; i++ {
			if i != (rezultatuKiekis - 1) {
				rezultatai[i] = rezultatai[i+1]
				continue
			}
			rezultatai[i] = AutoStruktura{pavadinimas: "", kiekis: 0}
		}

		rezultatuKiekis--
	}

	return rezultatai, rezultatuKiekis
}

// duomenu nuskaitymas is failo
func failoNuskaitymas(fileName string) ([]Salonas, []Pirkejas) {
	// atidaro faila ir patikrina klaidas
	file, err := os.Open(fileName)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	nuskaitymas := bufio.NewScanner(file)
	salonai := make([]Salonas, N)

	for i := 0; i < N; i++ {
		automobiliai := []Automobilis{}

		nuskaitymas.Scan()
		eilute := nuskaitymas.Text()

		stulpeliai := strings.Split(eilute, ",")
		salonai[i].pavadinimas = stulpeliai[0]
		prekiuKiekis, _ := strconv.Atoi(strings.Trim(stulpeliai[1], " "))

		for j := 0; j < prekiuKiekis; j++ {
			nuskaitymas.Scan()
			eilute := nuskaitymas.Text()

			stulpeliai := strings.Split(eilute, ",")

			pavadinimas := stulpeliai[0]
			galia, _ := strconv.Atoi(strings.Trim(stulpeliai[1], " "))
			kuroSanaudos, _ := strconv.ParseFloat(strings.Trim(stulpeliai[2], " "), 64)

			automobiliai = append(automobiliai, Automobilis{
				pavadinimas:  pavadinimas,
				galia:        galia,
				kuroSanaudos: kuroSanaudos})
		}
		salonai[i].automobiliai = automobiliai
		salonai[i].kiekis = prekiuKiekis
	}

	pirkejai := make([]Pirkejas, N)

	for i := 0; i < N; i++ {
		autoSarasas := []AutoStruktura{}

		nuskaitymas.Scan()
		eilute := nuskaitymas.Text()

		stulpeliai := strings.Split(eilute, ",")
		pirkejai[i].vardas = stulpeliai[0]
		pirkiniuKiekis, _ := strconv.Atoi(stulpeliai[1])

		for j := 0; j < pirkiniuKiekis; j++ {
			nuskaitymas.Scan()
			eilute := nuskaitymas.Text()

			stulpeliai := strings.Split(eilute, ",")

			pavadinimas := stulpeliai[0]

			kiekis, _ := strconv.Atoi(strings.Trim(stulpeliai[1], " "))
			autoSarasas = append(autoSarasas, AutoStruktura{
				pavadinimas: pavadinimas,
				kiekis:      kiekis})
		}
		pirkejai[i].automobiliai = autoSarasas
		pirkejai[i].kiekis = pirkiniuKiekis
	}

	return salonai, pirkejai
}

// atspausdina pradinius duomenis ir rezultatus i faila
func rezultatuSpausdinimasIFaila(salonai []Salonas, pirkejai []Pirkejas, galutiniaiRezultatai []AutoStruktura) {
	file, err := os.Create(RezultatuFailas)
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	isvedimas := bufio.NewWriter(file)

	fmt.Fprintln(isvedimas, "--------------------------- Pradiniai duomenys --------------------")

	for _, salonas := range salonai {
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		fmt.Fprintf(isvedimas, "| %40s   %20s |\n", salonas.pavadinimas, "Parduodama")
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		fmt.Fprintf(isvedimas, "| %7s | %27s | %10s | %10s |\n", "Eil nr.", "Modelis", "Galia", "Kaina")
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		for j, automobilis := range salonas.automobiliai {
			fmt.Fprintf(isvedimas, "| %7d | %27s | %10d | %10.2f |\n", j+1, automobilis.pavadinimas, automobilis.galia, automobilis.kuroSanaudos)
		}
	}

	for _, pirkejas := range pirkejai {
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		fmt.Fprintf(isvedimas, "| %30s   %30s |\n", pirkejas.vardas, "Perkama")
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		fmt.Fprintf(isvedimas, "| %7s | %40s | %10s |\n", "Eil nr.", "Modelis", "Kiekis")
		fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
		for j, automobiliai := range pirkejas.automobiliai {
			fmt.Fprintf(isvedimas, "| %7d | %40s | %10d |\n", j+1, automobiliai.pavadinimas, automobiliai.kiekis)
		}
	}
	fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
	fmt.Fprintln(isvedimas, "")

	fmt.Fprintln(isvedimas, "--------------------------- REZULTATAI ----------------------------")
	fmt.Fprintf(isvedimas, "| %7s | %40s | %10s |\n", "Eil nr.", "Modelis", "Kiekis")
	fmt.Fprintln(isvedimas, strings.Repeat("-", 67))
	for i, automobilis := range galutiniaiRezultatai {
		if automobilis.pavadinimas == "" && automobilis.kiekis == 0 {
			break
		}
		fmt.Fprintf(isvedimas, "| %7d | %40s | %10d |\n", i+1, automobilis.pavadinimas, automobilis.kiekis)
	}
	fmt.Fprintln(isvedimas, strings.Repeat("-", 67))

	isvedimas.Flush()
}
