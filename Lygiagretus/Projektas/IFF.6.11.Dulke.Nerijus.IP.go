// IFF-6/11 Nerijus Dulke IP
// Merge Sort

package main

import (
	"fmt"
	"math/rand"
	"sync"
	"time"
)

// pagrindinė funkcija
func main() {
	// duomenu skaicius
	count := 100
	// nuo kokio masyvo dydzio nustoti skaldyti i atskiras gijas
	// jei -1, skaldyti visada
	threshold := 30

	var start time.Time
	var elapsed time.Duration

	// uzpildomas masyvas su atsitiktiniais skaiciais
	arr := rand.Perm(count)

	fmt.Println("Rikiuojamo masyvo elementu skaicius:", count)
	fmt.Println("Nuo kokio masyvo dydzio nustoti skaldyti i atskiras gijas:", threshold)
	fmt.Println()

	// matuojamas lygiagretaus merge sort veikimo laikas
	start = time.Now()
	rikiuotas := parallelMergeSort(arr, threshold)
	elapsed = time.Since(start)
	fmt.Println("Lygiagretaus merge sort laikas:", elapsed)

	// matuojamas paprasto merge sort veikimo laikas
	start = time.Now()
	mergeSort(arr)
	elapsed = time.Since(start)
	fmt.Println("Paprasto merge sort laikas:", elapsed)

	// fmt.Println("Paprastas", arr)
	fmt.Println("Lygiagretus", rikiuotas)
}

// lygiagreti merge sort realizacija
func parallelMergeSort(arr []int, threshold int) []int {
	if len(arr) <= 1 {
		return arr
	}

	// masyvas suskaldomas i dvi dalis
	mid := len(arr) / 2
	left := arr[:mid]
	right := arr[mid:]

	// i atskiras gijas skaidyti tik iki tam tikro dydzio
	if threshold > 0 && len(arr) > threshold {
		var wg sync.WaitGroup
		wg.Add(2)

		// suskaldyi masyvai rekursiskai perduodami toliau skaldyti
		go func() {
			left = parallelMergeSort(left, threshold)
			wg.Done()
		}()
		go func() {
			right = parallelMergeSort(right, threshold)
			wg.Done()
		}()

		// laukiama kol gijos baigs darba
		wg.Wait()
	} else {
		// suskaldyi masyvai rekursiskai perduodami toliau skaldyti
		left = parallelMergeSort(left, threshold)
		right = parallelMergeSort(right, threshold)
	}

	// masyvai sujungiami ir surikiuojami
	return merge(left, right)
}

// paprasta merge sort realizacija
func mergeSort(arr []int) []int {
	if len(arr) <= 1 {
		return arr
	}

	// masyvas suskaldomas i dvi dalis
	mid := len(arr) / 2
	left := arr[:mid]
	right := arr[mid:]

	// suskaldyi masyvai rekursiskai perduodami toliau skaldyti
	left = mergeSort(left)
	right = mergeSort(right)

	// masyvai sujungiami ir surikiuojami
	return merge(left, right)
}

func merge(left []int, right []int) []int {
	lsize := len(left) - 1
	rsize := len(right) - 1
	size := lsize + rsize + 2
	i, j := 0, 0
	arr := make([]int, size)

	// rikiavimas ir jungimas
	for k := 0; k < size; k++ {
		if i > lsize && j <= rsize {
			arr[k] = right[j]
			j++
		} else if (j > rsize && i <= lsize) || (left[i] < right[j]) {
			arr[k] = left[i]
			i++
		} else {
			arr[k] = right[j]
			j++
		}
	}

	return arr
}
