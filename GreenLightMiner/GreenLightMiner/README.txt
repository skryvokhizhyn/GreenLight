* create venv
python -m venv .venv

* swtich to venv
if doesn't happen automatically by vscode:
source .venv/bin/activate

* pre-install mesa-dev for kivy:
sudo apt-get install libgl1-mesa-dev

* install requirements
pip install -r requirements.txt