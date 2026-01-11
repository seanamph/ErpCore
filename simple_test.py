import sys
print("Python is working!")
print(f"Version: {sys.version}")
print("Testing auto_clicker import...")
try:
    from auto_clicker import AutoClicker
    print("Import successful!")
    print("Creating instance...")
    clicker = AutoClicker()
    print(f"Instance created! Running: {clicker.running}")
except Exception as e:
    print(f"Error: {e}")
    import traceback
    traceback.print_exc()
