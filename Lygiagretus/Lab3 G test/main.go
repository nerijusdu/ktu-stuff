package main

import (
	"fmt"
)

func main() {
	fmt.Println("Start")

	var siuntejuKanalas = make(chan int)
	var lygSkKanalas = make(chan int)
	var nelygSkKanalas = make(chan int)
	var spausdinimoValdymoKanalas = make(chan bool)
	var pabaiga = make(chan bool)

	go s1(siuntejuKanalas)
	go s2(siuntejuKanalas)
	go spausdintojas(lygSkKanalas, spausdinimoValdymoKanalas, pabaiga)
	go spausdintojas(nelygSkKanalas, spausdinimoValdymoKanalas, pabaiga)
	go gavejas(siuntejuKanalas, lygSkKanalas, nelygSkKanalas, spausdinimoValdymoKanalas)

	<-pabaiga
	<-pabaiga
}

func gavejas(siuntejuKanalas <-chan int, lyginiaiSk chan<- int, nelyginiaiSk chan<- int, stop chan<- bool) {
	for i := 0; i < 20; i++ {
		var sk = <-siuntejuKanalas
		if sk%2 == 0 {
			lyginiaiSk <- sk
		} else {
			nelyginiaiSk <- sk
		}
	}
	stop <- true
	stop <- true
}

func spausdintojas(kanalas <-chan int, stop <-chan bool, done chan<- bool) {
	var arr = make([]int, 20)
	i := 0
	for {
		select {
		case <-stop:
			fmt.Println("-----Arr-----")
			for index := 0; index < i; index++ {
				fmt.Println(arr[index])
			}
			done <- true
			return
		case sk := <-kanalas:
			arr[i] = sk
			i++
		}
	}
}

func s1(kanalas chan<- int) {
	for i := 0; i <= 11; i++ {
		kanalas <- i
	}
}

func s2(kanalas chan<- int) {
	for i := 11; i >= 0; i-- {
		kanalas <- i
	}
}
