
uzd8([], Sum) :- format("~a~n", Sum).
uzd8([H|T], Sum) :-
    integer(H),
    NewSum is Sum + H,
    uzd8(T, NewSum)
    ;
    not(integer(H)),
    uzd8(T, Sum).

isReverse(List) :-
    reverse(List, List),
    string_codes(Str, List),
    format("~a~n", Str).
isReverse(_).

row([]).
row([H|T]) :-
    string_codes(H, Chars),
    isReverse(Chars),
    row(T).

uzd9([]).
uzd9([H|T]) :-
    row(H),
    uzd9(T).

start :-
    writeln('8 uzd atsakymas:'),
    Numbers = [1, 2, 4, 1.3, 4.5, 3],
    uzd8(Numbers, 0),
    writeln('9 uzd atsakymas:'),
    Words = [["aba", "bbb", "ca"],["ds","eegee","fa"]],
    uzd9(Words).
