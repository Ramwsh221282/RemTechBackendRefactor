namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public delegate Result ResultFn();
public delegate Result<T> ResultFn<T>();
public delegate Task<Result<T>> AsyncResultFn<T>(CancellationToken ct = default);
public delegate Task<Result> AsyncResultFn(CancellationToken ct = default);
