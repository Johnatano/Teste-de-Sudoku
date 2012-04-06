% Autor:Johnatan Alves de Oliveira & Stéfany Rodrigues
% Data: 4/6/2012

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Funções Auxiliares
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%Retorna o tamanho de uma lista
tamanho([],Resultado) :- Resultado is 0.
tamanho([A | B],Resultado) :- tamanho(B,R), Resultado is R + 1.

%Retorna o maior elemento de uma lista
max([],Resultado) :- Resultado is -10000.
max([A | B],Resultado) :- max(B, R), (A >= R, Resultado is A ; Resultado is R), !.

%Retorna o menor elemento de uma lista
min([],Resultado) :- Resultado is 100000.
min([A | B],Resultado) :- min(B, R), (A >= R, Resultado is R ; Resultado is A), !.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Funções de Pertinência
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

triangular(X,[A,B,C],Resultado) :- min([(X-A)/(B-A), (C-X)/(C-B)], M), max([M,0],R), Resultado is R.
trapezoidal(X,[A,B,C,D],Resultado) :- min([(X-A)/(B-A), 1, (D-X)/(D-C)], M),  max([M,0],R), Resultado is R.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Calcula o centro de um polígono
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

centroideA(Tam,Conj,[],Resultado) :- Resultado is 0.
centroideA(Tam,Conj,[A | B],Resultado) :-
     centroideA(Tam, Conj, B, Res),
     (Tam =:= 4, trapezoidal(A,Conj,Tmp),! ; triangular(A,Conj,Tmp),!),
     Resultado is Res + A*Tmp.

centroideB(Tam, Conj,[],Resultado) :- Resultado is 0.
centroideB(Tam, Conj,[A | B],Resultado) :-
     centroideB(Tam, Conj, B, Res),
     (Tam =:= 4, trapezoidal(A,Conj,Tmp),! ; triangular(A,Conj,Tmp),!),
     Resultado is Res + Tmp.

centroide(Conj, Resultado) :-
     tamanho(Conj, Tam),
     centroideA(Tam,Conj,Conj,A),
     centroideB(Tam,Conj,Conj,B),
     (B > 0, Resultado is A / B ; Resultado is 0).

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Conjuntos Fuzzy
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%Delimitação dos conjuntos

ruim([0,1,2,3]).
medio([2,4,6]).
bom([4,6,8]).
excelente([6,8,10,10]).

gorjetaRuim([0,1,3,6]).
gorjetaMedia([3,6,9]).
gorjetaBom([6,9,12]).
gorjetaExcelente([9,12,15,15]).


%Funções de pertinência para cada conjunto
fn_comida_ruim(X,P) :- ruim(Conj), trapezoidal(X,Conj,P).
fn_comida_medio(X,P) :- medio(Conj), triangular(X,Conj,P).
fn_comida_bom(X,P) :- bom(Conj), triangular(X,Conj,P).
fn_comida_excelente(X,P) :- excelente(Conj), trapezoidal(X,Conj,P).

fn_atendimento_ruim(X,P) :- ruim(Conj), trapezoidal(X,Conj,P).
fn_atendimento_medio(X,P) :- medio(Conj), triangular(X,Conj,P).
fn_atendimento_bom(X,P) :- bom(Conj), triangular(X,Conj,P).
fn_atendimento_excelente(X,P) :- excelente(Conj), trapezoidal(X,Conj,P).


fn_gorjeta_ruim(X,P) :- gorjetaRuim(Conj), trapezoidal(X,Conj,P).
fn_gorjeta_medio(X,P) :- gorjetaMedio(Conj), triangular(X,Conj,P).
fn_gorjeta_bom(X,P) :- gorjetaBom(Conj), triangular(X,Conj,P).
fn_gorjeta_excelente(X,P) :- gorjetaExcelente(Conj), trapezoidal(X,Conj,P).




%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Variável Linquística
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

qualidadeAtendimento(X,Pertinencia) :- fn_atendimento_ruim(X,Ru),
fn_atendimento_medio(X,Me), fn_atendimento_bom(X,B), fn_atendimento_excelente(X,Exc),
 Pertinencia = [Ru,Me,B,Exc].

qualidadeComida(X,Pertinencia) :- fn_comida_ruim(X,Ru),
fn_comida_medio(X,Me), fn_comida_bom(X,B), fn_comida_excelente(X,Exc),
 Pertinencia = [Ru,Me,B,Exc].

gorjeta(X,Pertinencia) :- fn_gorjeta_ruim(X,Rui),
fn_gorjeta_medio(X,M), fn_gorjeta_bom(X,Bo), fn_gorjeta_excelente(X,Ex),
 Pertinencia = [Rui,M,Bo,Ex].

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Regras de inferência e implicação
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% Se comida=ruim ou serviço=ruim entao gorjeta=ruim
% Se comida=boa ou serviço=bom entao gorjeta=excelente
% Se comida=boa ou serviço=medio entao gorjeta=medio
% Se comida=media ou serviço=bom entao gorjeta=media




%Validas
%se atendimento=excelente,comida=excelente entao gorjeta =excelente
regra1([Aruim,Amedio,Abom,Aexcelente],[Cruim,Cmedio,Cbom,Cexcelente],[Gruim,Gmedio,Gbom,Gexcelete]) :- min(Aexcelente,Cexcelente,Resultado), Gexcelente is Resultado.

%se atendimento=bom,comida=bom entao gorjeta = bom
regra2([Aruim,Amedio,Abom,Aexcelente],[Cruim,Cmedio,Cbom,Cexcelente],[Gruim,Gmedio,Gbom,Gexcelete]) :- min(Amedio,Cmedio,Resultado), Gmedio is Resultado.

%se atendimento=medio,comida=medio entao gorjeta = media
regra3([Aruim,Amedio,Abom,Aexcelente],[Cruim,Cmedio,Cbom,Cexcelente],[Gruim,Gmedio,Gbom,Gexcelete]) :- min(Aexcelente,Cexcelente,Resultado), Gexcelente is Resultado.

%se atendimento=ruim,comida=ruim entao gorjeta = ruim
regra4([Aruim,Amedio,Abom,Aexcelente],[Cruim,Cmedio,Cbom,Cexcelente],[Gruim,Gmedio,Gbom,Gexcelete]) :- min(Aexcelente,Cexcelente,Resultado), Gexcelente is Resultado.


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Defuzzyficação
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

defuzzyficacao([Aruim,Amedio,Abom,Aexcelente],[Cruim,Cmedio,Cbom,Cexcelente],[Gruim,Gmedio,Gbom,Gexcelete]Resultado) :-
       ruim(Ruim),centroide(Ruim,RuOut),
       medio(Medio),centroide(Medio,MeOut),
       bom(Bom),centroide(Bom,BOut),
       excelente(Excelente),centroide(Excelente,ExOut),
       Resultado is RuOut*Ru + Me*MeOut + B*BOut + Ex*ExOut, !.

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Controlador fuzzy
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

controladorFuzzy(Comida,Atendimento,Gorjeta) :-

      %Fuzzyficação
      gorjeta(Comida,Atendimento,Gor),

      % Inferência - Implicação e Agregação
      regra1(Pertinencias,Ruim),
      regra2(Pertinencias,Medio),
      regra3(Pertinencias,Bom),
      regra4(Pertinencias,Excelente),

      %Defuzzyficação
      defuzzyficacao([Ruim,Medio,Bom,Excelente],Gorjeta), !.

