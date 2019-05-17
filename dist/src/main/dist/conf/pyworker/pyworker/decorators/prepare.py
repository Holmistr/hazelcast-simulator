def prepare(func):
    func._prepare = True
    return func