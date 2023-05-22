<?php

namespace App\Console\Commands;

use Illuminate\Console\Command;
use Illuminate\Support\Facades\Cache;

class ClearSessionCommand extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'session:clear';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Clear all session data';

    /**
     * Execute the console command.
     *
     * @return void
     */
    public function handle()
    {
        // 清除所有會話資料
        Cache::flush();

        $this->info('All session data cleared successfully.');
    }
}
