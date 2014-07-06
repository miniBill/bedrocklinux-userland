all: brm.cpp parser.cpp filesystem.cpp
	$(CXX) -std=c++11 -Wall -Wno-unused-function $^ -o brm -static -g -ggdb

clean:
	-rm brm

install:
	mkdir -p $(prefix)/bin
	install -m 755 brm $(prefix)/bin/brm

uninstall:
	rm $(prefix)/bin/brm
