<?php

use App\Models\Service;
use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('therapist_open_services', function (Blueprint $table) {
            $table->id();
            $table->foreignIdFor(Service::class)->constrained()
                ->cascadeOnUpdate()
                ->cascadeOnDelete();
            $table->string('user_id');
            $table->foreign('user_id')->references('staff_id')->on('users')->cascadeOnUpdate()->restrictOnDelete();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::table('therapist_open_services', function (Blueprint $table) {
            $table->dropForeign(['user_id']);
            $table->dropForeign(['service_id']);
        });
        Schema::dropIfExists('therapist_open_services');
    }
};