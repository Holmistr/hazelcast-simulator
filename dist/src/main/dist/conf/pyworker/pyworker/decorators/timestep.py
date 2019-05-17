def timestep(func):
    func._timestep = True
    return func