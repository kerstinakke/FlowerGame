using System;

public class GeneCombiner {
    const int GENE_W = 12; // number of bits in a gene -- this may be tuned
    static Random rng = new Random();

    public struct Gene
    {
        public bool[] dna;
        
        public int popCount ()
        {
            // calculates the number of ones in the given bitstring
            int result = 0;
            for (int i = 0; i < GENE_W; i++) {
                if (dna[i]) {
                    result++;
                }
            }
            return result;
        }
    }

    public static Gene randomGene()
    {
        Gene result = new Gene();
        result.dna = new bool [GENE_W];
        
        for (int i = 0; i < GENE_W; i++) {
            result.dna[i] = (rng.Next(0, 2) == 0);
        }
        
        return result;
    }
    
    const double TARGET_PROB = 0.2f;
    public static Gene targetGene()
    {
        // genes of target flowers should be "extreme"
        Gene result = new Gene();
        result.dna = new bool [GENE_W];
        
        for (int i = 0; i < GENE_W; i++) {
            if (rng.NextDouble() < TARGET_PROB) {
                result.dna[i] = true;   
            }
        }
        
        if (rng.Next(0, 2) == 0) {
            for (int i = 0; i < GENE_W; i++) {
                result.dna[i] = !result.dna[i];
            }
        }
        
        return result;
    }

    public static Gene cross(Gene p, Gene q)
    {
        Gene result = new Gene();
        result.dna = new bool [GENE_W];
        
        for (int i = 0; i < GENE_W; i++) {
            if (rng.Next(0, 2) == 0) {
                result.dna[i] = p.dna[i];
            } else {
                result.dna[i] = q.dna[i];
            }
        }
    
        // shuffle the gene
        for (int i = GENE_W - 1; i > 0; i--) {
            int swapWith = rng.Next(0, i);
            bool temp = result.dna[i];
            result.dna[i] = result.dna[swapWith];
            result.dna[swapWith] = temp;
        }
        return result;
    }

    const double FLIP_PROB = 0.2f;
    public static Gene mutate(Gene p)
    {
        Gene result = new Gene();
        result.dna = new bool [GENE_W];
        
        for (int i = 0; i < GENE_W; i++) {
            if (rng.NextDouble() < FLIP_PROB) {
                result.dna[i] = !p.dna[i];
            } else {
                result.dna[i] = p.dna[i];
            }
        }
        
        // shuffle the gene
        for (int i = GENE_W - 1; i > 0; i--) {
            int swapWith = rng.Next(0, i);
            bool temp = result.dna[i];
            result.dna[i] = result.dna[swapWith];
            result.dna[swapWith] = temp;
        }
        return result;
    }
}