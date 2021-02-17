; JESS aplinkoje komentarus pasalinkite
;
;(clear)

(deftemplate pele (slot spalva) (slot kiekis) )
(deftemplate katino (slot busena) (slot suvalgyta_peliu) )

(deffacts faktu-inicializavimas
  (pele (spalva pilka) (kiekis 5))
  (pele (spalva balta) (kiekis 3))
  (katino (busena "alkanas") (suvalgyta_peliu 0))
)

(defrule r1 "Kai katinas alkanas, jis nori valgyti"
  ?fact-id <- (katino (busena ?busena))  
  (test (eq ?busena "alkanas"))
  =>
  (modify ?fact-id (busena "nori valgyti"))
)
(defrule r2 "Kai katinas nori valgyti ir yra peliu, jis valgo peles"
  ?fact-id1 <- (katino (busena "nori valgyti") (suvalgyta_peliu ?suvalgyta))
  ?fact-id2 <- (pele (spalva ?spalva) (kiekis ?kiekis))
  (test (> ?kiekis 0))
  =>
  
  (if (eq ?spalva balta) then (printout t "py-py!" crlf)
                         else (printout t "pyyyyy" crlf))
  (modify ?fact-id2 (kiekis (- ?kiekis 1))  )
  
  (modify ?fact-id1 (suvalgyta_peliu (+ ?suvalgyta 1)) ) 
  (printout t "miau" crlf)
)

(defrule r3 "kai katinas suvalgo 5 peles, jis tampa storu katinu"
  (declare (salience 10))
  ?fact-id1 <- (katino (busena "nori valgyti") (suvalgyta_peliu ?suvalgyta))
  (test (= ?suvalgyta 5)) 
  
=>
  (modify ?fact-id1 (busena "storas"))
)

; JESS aplinkoje komentarus pasalinkite
;
; (reset)
; (facts)
; (watch all)
; (run)
