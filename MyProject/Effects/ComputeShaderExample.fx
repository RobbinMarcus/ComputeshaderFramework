static const int nStructs = 10000;
static const int nThreads = 50;


struct ExampleStruct
{
	float3 Data;
};

RWStructuredBuffer<ExampleStruct> data : register(t0);

// Main
[numthreads(nThreads, 1, 1)]
void CSMain(uint3 id : SV_DispatchThreadID)
{
	int range = nStructs / nThreads;
	for (uint i = id.x * range; i < id.x * range + range; i++)
	{
		int result = 0;
		for (uint j = 0; j < nStructs; j++)
		{
			result += j;
		}
		data[i].Data = float3(result, result, result);
	}
}