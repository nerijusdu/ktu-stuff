# IFF-6/11 Nerijus Dulke Lab1
# https://uva.onlinejudge.org/index.php?option=com_onlinejudge&Itemid=8&category=9&page=show_problem&problem=725

from Maze import Maze

mazes = []

duom = open('duom.txt')

mazeCount = int(duom.readline())

i = 0
iterations = 0
while i < mazeCount:
  maze = Maze()
  line = duom.readline()

  while line and not line.startswith('_'):
    maze.addline(line)
    iterations = iterations + 1
    line = duom.readline()

  mazes.append(maze)
  i = i + 1

duom.close()

rez = open('rez.txt', 'w+')

for maze in mazes:
  maze.paint()
  maze.printresult(rez)
  rez.write('_____\n')

rez.close()
