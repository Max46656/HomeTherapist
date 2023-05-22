<?php

namespace Database\Seeders;

use App\Models\Order;
use Illuminate\Database\Seeder;

class OrderSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        $totalRecords = 10000;
        $batchSize = 1000;
        $totalBatches = ceil($totalRecords / $batchSize);
        for ($i = 0; $i < $totalBatches; $i++) {
            Order::factory()->count($batchSize)->create();
            sleep(0.1);
        }
    }
}
