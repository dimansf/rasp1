import subprocess, time



def var2():
	print('var2')

	subprocess.run([r"C:\Users\dimansf\source\repos\Raspil\ClientRaspil\bin\Debug\ClientRaspil.exe",  '3', r"C:\Users\dimansf\source\repos\Raspil\ClientRaspil\resources\json2.txt"])
	time.sleep(2)
	input()
	
def var1():
	print('var1')
	while(1):
		subprocess.run([r"C:\Users\dimansf\source\repos\Raspil\ClientRaspil\bin\Debug\ClientRaspil.exe",  '3', r"C:\Users\dimansf\source\repos\Raspil\ClientRaspil\resources\json2.txt"])
	
var1()