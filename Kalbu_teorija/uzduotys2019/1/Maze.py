WALL = 'X'
EMPTY = ' '
MARKED = '#'
START = '*'
directions = ['u', 'd', 'l', 'r', 'ul', 'ur', 'dl', 'dr']

class Maze:
  def __init__(self):
    self.lines = []
    self.startX = -1
    self.startY = -1
    self.maxX = -1
    self.maxY = -1
  
  def addline(self, line):
    self.lines.append(line)

    if self.startX == -1:
      index = line.find(START)
      if index != -1:
        self.startX = index
        self.startY = self.count() - 1
    
    self.maxY = self.count() - 1
    maxX = len(line) - 1
    if maxX > self.maxX:
      self.maxX = maxX
    return

  def printlines(self):
    for line in self.lines:
      print line
    return
  
  def count(self):
    return len(self.lines)

  def printresult(self, file):
    file.writelines(self.lines)
    return
  
  def getvalue(self, coord):
    return self.lines[coord.y][coord.x]
  
  def markvalue(self, coord):
    if self.getvalue(coord) is EMPTY:
      self.lines[coord.y] = self.lines[coord.y][:coord.x] + MARKED + self.lines[coord.y][coord.x + 1:]
      return True
    return False
  
  def paint(self):
    self.max = Coord(self.maxX, self.maxY)
    current = Coord(self.startX, self.startY)
    coordsToSearch = [current]
    visited = []
    
    while len(coordsToSearch) > 0:
      current = Coord(coordsToSearch[0].x, coordsToSearch[0].y)
      del coordsToSearch[0]

      for direction in directions:
        neighbour = self.getNeighbour(current, direction)
        isVisited = filter(lambda x: neighbour.equals(x), visited)

        if neighbour is None or len(isVisited) > 0:
          continue

        success = self.markvalue(neighbour)
        if success:
          coordsToSearch.append(neighbour)

      visited.append(current)
    return

  def getNeighbour(self, current, direction):
    coord = Coord(current.x, current.y)
    if not coord.canmove(direction, self.max):
      return None

    if direction is 'u':
      coord.up()
    elif direction is 'd':
      coord.down()
    elif direction is 'l':
      coord.left()
    elif direction is 'r':
      coord.right()
    elif direction is 'ul':
      coord.up()
      coord.left()
    elif direction is 'ur':
      coord.up()
      coord.right()
    elif direction is 'dl':
      coord.down()
      coord.left()
    elif direction is 'dr':
      coord.down()
      coord.right()

    return coord


class Coord:
  def __init__(self, x, y):
    self.x = x
    self.y = y
  
  def canmove(self, direction, maxcoord):
    if direction is 'u':
      return self.y != 0
    elif direction is 'd':
      return self.y != maxcoord.y
    elif direction is 'l':
      return self.x != 0
    elif direction is 'r':
      return self.x != maxcoord.x
    elif direction is 'ul':
      return self.y != 0 and self.x != 0
    elif direction is 'ur':
      return self.y != 0 and self.x != maxcoord.x
    elif direction is 'dl':
      return self.y != maxcoord.y and self.x != 0
    elif direction is 'dr':
      return self.y != maxcoord.y and self.x != maxcoord.x
    return False
  
  def up(self):
    self.y = self.y - 1
  
  def down(self):
    self.y = self.y + 1
  
  def left(self):
    self.x = self.x - 1
  
  def right(self):
    self.x = self.x + 1
  
  def equals(self, other):
    return self.x == other.x and self.y == other.y