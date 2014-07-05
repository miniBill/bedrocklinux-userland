all: brm.c trie.c trie.h
	$(CC) -Wall brm.c trie.c -o brm -static

clean:
	-rm brm

install:
	mkdir -p $(prefix)/bin
	install -m 755 brm $(prefix)/bin/brm

uninstall:
	rm $(prefix)/bin/brm
