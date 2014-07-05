all: brm.c
	$(CC) -Wall brm.c -o brm -static

clean:
	-rm brm

install:
	mkdir -p $(prefix)/bin
	install -m 755 brm $(prefix)/bin/brm

uninstall:
	rm $(prefix)/bin/brm
