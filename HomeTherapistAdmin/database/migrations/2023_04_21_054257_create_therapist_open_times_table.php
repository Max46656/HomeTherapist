<?php

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
        Schema::create('therapist_open_times', function (Blueprint $table) {
            $table->id();
            $table->string('user_id');
            $table->foreign('user_id')->references('staff_id')->on('users')->cascadeOnUpdate()->restrictOnDelete();
            $table->dateTime('start_dt')->nullable();
            $table->foreign('start_dt')->references('dt')->on('calendar');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::table('therapist_open_times', function (Blueprint $table) {
            $table->dropForeign('therapist_open_times_user_id_foreign');
            $table->dropForeign(['start_dt']);
        });
        Schema::dropIfExists('therapist_open_times');
    }
};
